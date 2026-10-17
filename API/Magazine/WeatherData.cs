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

public interface IWeatherData
{
    public Forecast[] Forecasts { get; set; } 
    public Dictionary<string, int> Temperatures { get; set; }
    public DateTime LastRefreshUTC { get; set; }
    public bool IsValid { get; set; }
}

public class WeatherData : IWeatherData
{
    public Forecast[] Forecasts { get; set; } = new Forecast[4];
    public Dictionary<string, int> Temperatures { get; set; } = [];
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
            MaxTemp = todayForecast?.SelectSingleNode(".//span[contains(@class, 'wr-day-temperature__high-value')]//span[contains(@class, 'wr-value--temperature--c')]")?.InnerText[..^1].Trim();
            MinTemp = todayForecast?.SelectSingleNode(".//span[contains(@class, 'wr-day-temperature__low-value')]//span[contains(@class, 'wr-value--temperature--c')]")?.InnerText[..^1].Trim();

            Sunrise = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'wr-c-environment-day-wrapper--active')]//span[contains(@class, 'wr-c-astro-data__sunrise')]/span[contains(@class, 'wr-c-astro-data__time')]")?.InnerText.Trim();
            Sunset = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'wr-c-environment-day-wrapper--active')]//span[contains(@class, 'wr-c-astro-data__sunset')]/span[contains(@class, 'wr-c-astro-data__time')]")?.InnerText.Trim();

            for(int i=0; i<=2; i++) { AddForecast(doc, i); }
            
            LastRefreshUTC = DateTime.UtcNow;
            Temperatures.Add("WeatherLondon", GetTemperature(cc, CeefaxSectionType.WeatherLondon));
            Temperatures.Add("WeatherCardiff", GetTemperature(cc, CeefaxSectionType.WeatherCardiff));
            Temperatures.Add("WeatherEdinburgh", GetTemperature(cc, CeefaxSectionType.WeatherEdinburgh));
            Temperatures.Add("WeatherTruro", GetTemperature(cc, CeefaxSectionType.WeatherTruro));
            Temperatures.Add("WeatherLerwick", GetTemperature(cc, CeefaxSectionType.WeatherLerwick));
            Temperatures.Add("WeatherManchester", GetTemperature(cc, CeefaxSectionType.WeatherManchester));
            Temperatures.Add("WeatherBelfast", GetTemperature(cc, CeefaxSectionType.WeatherBelfast));
            
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

    private int GetTemperature(CeefaxContent cc, CeefaxSectionType city)
    {
        string html = String.Empty;
        TextReader tr = new StringReader(cc.UriCache.Find(l => l.Location == cc.Sections.Find(z => z.Name == city)!.Feed)!.ContentString!);
        SyndicationFeed feed = SyndicationFeed.Load(XmlReader.Create(tr));
        int temperature = Convert.ToInt32(feed.Items.First().Summary.Text.Split("°C")[0].Split(" ")[^1]);

        return temperature;
    }
}