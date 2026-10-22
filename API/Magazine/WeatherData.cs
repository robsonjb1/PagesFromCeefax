using System.ServiceModel.Syndication;
using System.Xml;
using API.Architecture;
using HtmlAgilityPack;
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
}

public class WeatherData
{
    public Forecast[] Forecasts { get; set; } = new Forecast[4];
    public Dictionary<string, SpotTemperature> Locations { get; set; } = [];
    public DateTime LastRefreshUTC { get; set; }
    public string Sunrise { get; set; }
    public string Sunset { get; set; }
    public string MaxTemp { get; set; }
    public string MinTemp { get; set; }
    public bool IsValid { get; set; } = false;

    public WeatherData(CeefaxContent cc)
    {
        try
        {
            string html = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.Weather).Feed).ContentString;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode todayForecast = doc.DocumentNode.SelectSingleNode("//a[contains(@class, 'wr-day__content')][.//span[contains(@class, 'wr-date') and normalize-space(.)='Today']]");
            
            if(todayForecast == null)
            {
                todayForecast = doc.DocumentNode.SelectSingleNode("//a[contains(@class, 'wr-day__content')][.//span[contains(@class, 'wr-date') and normalize-space(.)='Tonight']]");   
            }
            
            MaxTemp = todayForecast?.SelectSingleNode(".//span[contains(@class, 'wr-day-temperature__high-value')]//span[contains(@class, 'wr-value--temperature--c')]")?.InnerText[..^1].Trim() ?? "n/a";
            MinTemp = todayForecast?.SelectSingleNode(".//span[contains(@class, 'wr-day-temperature__low-value')]//span[contains(@class, 'wr-value--temperature--c')]")?.InnerText[..^1].Trim() ?? "n/a";

            Sunrise = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'wr-c-environment-day-wrapper--active')]//span[contains(@class, 'wr-c-astro-data__sunrise')]/span[contains(@class, 'wr-c-astro-data__time')]")?.InnerText.Trim();
            Sunset = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'wr-c-environment-day-wrapper--active')]//span[contains(@class, 'wr-c-astro-data__sunset')]/span[contains(@class, 'wr-c-astro-data__time')]")?.InnerText.Trim();

            for(int i=0; i<=2; i++) { AddForecast(doc, i); }
            
            LastRefreshUTC = DateTime.UtcNow;
            Locations.Add("Heathfield", GetTemperature(cc, CeefaxSectionType.WeatherHeathfield));
            Locations.Add("Swansea", GetTemperature(cc, CeefaxSectionType.WeatherSwansea));
            Locations.Add("Edinburgh", GetTemperature(cc, CeefaxSectionType.WeatherEdinburgh));
            Locations.Add("Penzance", GetTemperature(cc, CeefaxSectionType.WeatherPenzance));
            Locations.Add("Inverness", GetTemperature(cc, CeefaxSectionType.WeatherInverness));
            Locations.Add("Manchester", GetTemperature(cc, CeefaxSectionType.WeatherManchester));
            Locations.Add("Belfast", GetTemperature(cc, CeefaxSectionType.WeatherBelfast));
            Locations.Add("Cambridge", GetTemperature(cc, CeefaxSectionType.WeatherCambridge));
            
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
        int temperature = Convert.ToInt32(feed.Items.First().Summary.Text.Split("°C")[0].Split(" ")[^1]);
        int.TryParse(feed.Items.First().Summary.Text.Split("mph")[0].Split(" ")[^1], out int windSpeed);
    
        return new SpotTemperature() { Temperature = temperature, WindSpeed = windSpeed };
    }
}