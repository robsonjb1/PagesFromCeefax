using System.Collections;
using System.ServiceModel.Syndication;
using System.Xml;
using API.Architecture;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Serilog;

namespace API.Magazine;

public class Forecast
{
    public string Title { get; set; }
    public string Body { get; set; }
}

public class SpotTemperature
{
    public int Temperature { get; set; }
    public int WindSpeed { get; set; }
    public string WindDirection { get; set; }
    public int Pressure { get; set; }
    public bool PressureFalling { get; set; }
    public bool PressureRising { get; set; }
}

public class WeatherData
{
    public Forecast[] Forecasts { get; set; } = new Forecast[4];
    public Dictionary<string, SpotTemperature> Locations { get; set; } = [];
    public DateTime LastRefreshUTC { get; set; }
    public string Sunrise { get; set; }
    public string Sunset { get; set; }
    public int MaxTemp { get; set; }
    public int MinTemp { get; set; }
    public string MaxTempLabel { get; set; }
    public string MinTempLabel { get; set; }
    public bool IsValid { get; set; } = false;

    public WeatherData(CeefaxContent cc)
    {
        try
        {
            // Get UK forecast
            string html = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.WeatherUK).Feed).ContentString;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            for(int i=0; i<=2; i++) { AddForecast(doc, i); }
            
            // Get spot temperatures
            html = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.WeatherHeathfield).Feed).ContentString;
            doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode todayForecast = doc.DocumentNode.SelectSingleNode("//a[contains(@class, 'wr-day__content')][.//span[contains(@class, 'wr-date') and normalize-space(.)='Today']]");
            if(todayForecast == null)
            {
                todayForecast = doc.DocumentNode.SelectSingleNode("//a[contains(@class, 'wr-day__content')][.//span[contains(@class, 'wr-date') and normalize-space(.)='Tonight']]");   
            }

            MaxTempLabel = todayForecast?.SelectSingleNode(".//span[contains(@class, 'wr-day-temperature__high-value')]//span[contains(@class, 'wr-value--temperature--c')]")?.InnerText[..^1].Trim() ?? "n/a";
            MinTempLabel = todayForecast?.SelectSingleNode(".//span[contains(@class, 'wr-day-temperature__low-value')]//span[contains(@class, 'wr-value--temperature--c')]")?.InnerText[..^1].Trim() ?? "n/a";
            
            int.TryParse(MaxTempLabel, out int max);
            int.TryParse(MinTempLabel, out int min);

            if(MaxTempLabel.EndsWith("n/a")) // Avoid n/a being coloured cyan
            {
                max = 10;
            }
            else
            {
                MaxTempLabel += " C";
            }

            if(MinTempLabel.EndsWith("n/a"))
            {
                min = 10;
            }
            else
            {
                MinTempLabel += " C";
            }

            MaxTemp = max;
            MinTemp = min;
            
            Sunrise = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'wr-c-environment-day-wrapper--active')]//span[contains(@class, 'wr-c-astro-data__sunrise')]/span[contains(@class, 'wr-c-astro-data__time')]")?.InnerText.Trim();
            Sunset = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'wr-c-environment-day-wrapper--active')]//span[contains(@class, 'wr-c-astro-data__sunset')]/span[contains(@class, 'wr-c-astro-data__time')]")?.InnerText.Trim();

            Sunrise ??= "n/a";
            Sunset ??= "n/a";

            LastRefreshUTC = DateTime.UtcNow;
            Locations.Add("Heathfield", GetTemperature(cc, CeefaxSectionType.WeatherSpotHeathfield));
            Locations.Add("Swansea", GetTemperature(cc, CeefaxSectionType.WeatherSpotCaerphilly));
            Locations.Add("Edinburgh", GetTemperature(cc, CeefaxSectionType.WeatherSpotLivingston));
            Locations.Add("Penzance", GetTemperature(cc, CeefaxSectionType.WeatherSpotNewquay));
            Locations.Add("Inverness", GetTemperature(cc, CeefaxSectionType.WeatherSpotInverness));
            Locations.Add("Manchester", GetTemperature(cc, CeefaxSectionType.WeatherSpotMiddleton));
            Locations.Add("Belfast", GetTemperature(cc, CeefaxSectionType.WeatherSpotBelfast));
            Locations.Add("Cambridge", GetTemperature(cc, CeefaxSectionType.WeatherSpotCambridge));
            Locations.Add("Birmingham", GetTemperature(cc, CeefaxSectionType.WeatherSpotWolverhampton));

            IsValid = true;
        }
        catch (Exception ex)
        {
            Log.Fatal($"WEATHERDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }

    private void AddForecast(HtmlDocument doc, int forecastNo)
    {
        Forecasts[forecastNo] = new Forecast() {
            Title = doc.DocumentNode.SelectSingleNode($"(//p[@class='ssrcss-1q0x1qg-Paragraph e1jhz7w10'])[{2+(forecastNo*2)}]")?.InnerText,
            Body = doc.DocumentNode.SelectSingleNode($"(//p[@class='ssrcss-1q0x1qg-Paragraph e1jhz7w10'])[{3+(forecastNo*2)}]")?.InnerText
        };
    }

    private SpotTemperature GetTemperature(CeefaxContent cc, CeefaxSectionType city)
    {
        string html = String.Empty;
        TextReader tr = new StringReader(cc.UriCache.Find(l => l.Location == cc.Sections.Find(z => z.Name == city)!.Feed)!.ContentString!);
        SyndicationFeed feed = SyndicationFeed.Load(XmlReader.Create(tr));
        int.TryParse(feed.Items.First().Summary.Text.Split("°C")[0].Split(" ")[^1], out int temperature);
        int.TryParse(feed.Items.First().Summary.Text.Split("mb")[0].Split(" ")[^1], out int pressure);
        int.TryParse(feed.Items.First().Summary.Text.Split("mph")[0].Split(" ")[^1], out int windSpeed);

        string[] compassWords = feed.Items.First().Summary.Text.Split("Wind Direction: ")[1].Split(", ")[0].Split(" ");
        string windDirection = windSpeed.ToString() + "mph ";
        foreach(string compass in compassWords)
        {
            string letter = compass[..1].ToUpper();
            if(letter == "N" || letter == "S" || letter == "E" || letter == "W")
            {
                windDirection += letter;
            }
        }

        bool pressureFalling = feed.Items.First().Summary.Text.Contains("Falling");
        bool pressureRising = feed.Items.First().Summary.Text.Contains("Rising");

        return new SpotTemperature() { Temperature = temperature, WindDirection = windDirection, WindSpeed = windSpeed, Pressure = pressure, PressureFalling = pressureFalling, PressureRising = pressureRising };
    }
}