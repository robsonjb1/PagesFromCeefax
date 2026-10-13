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
    public bool IsValid { get; set; } = false;

    public WeatherData(CeefaxContent cc)
    {
        try
        {
            string html = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.Weather).Feed).ContentString;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

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