using System.ComponentModel;
using System.Text.Json;
using API.Architecture;
using API.DataTransferObjects;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public class Forecast
{
    public string Title { get; set; }
    public string Body { get; set; }
}
public class CityTemperature
{
    public int CurrentTemp { get; set; }
    public int MaxTemp { get; set; }
    public int MinTemp { get; set; }
    public string Description { get; set; }
}
public interface IWeatherData
{
    public Forecast[] Forecasts { get; set; } 
    public Dictionary<string, CityTemperature> Temperatures { get; set; }
    public DateTime LastRefreshUTC { get; set; }
    public bool IsValid { get; set; }
}

public class WeatherData : IWeatherData
{
    public Forecast[] Forecasts { get; set; } = new Forecast[4];
    public Dictionary<string, CityTemperature> Temperatures { get; set; } = new();
    public DateTime LastRefreshUTC { get; set; }
    public bool IsValid { get; set; } = false;

    public WeatherData(ICeefaxContent cc)
    {
        try
        {
            string html = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.Weather).Feed).ContentString;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            for(int i=1; i<=3; i++) { AddForecast(doc, i); }
            
            LastRefreshUTC = DateTime.UtcNow;
            SystemConfig.WeatherCities.ForEach(z => Temperatures.Add(z, GetTempFromApiResponse(cc, z)));
            
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
            Title = doc.DocumentNode.SelectSingleNode($"(//b[@class='ssrcss-1xjjfut-BoldText e5tfeyi3'])[{forecastNo}]").InnerText,
            Body = doc.DocumentNode.SelectSingleNode($"(//b[@class='ssrcss-1xjjfut-BoldText e5tfeyi3'])[{forecastNo}]/parent::p/following-sibling::p").InnerText
        };
    }

    private CityTemperature GetTempFromApiResponse(ICeefaxContent cc, string city)
    {
        string json = String.Empty;
        try
        {
            json = cc.UriCache.First(l => l.Tag == city).ContentString;
            OpenWeather dto = JsonSerializer.Deserialize<OpenWeather>(json);
            return new CityTemperature 
            {
                CurrentTemp = Convert.ToInt32(dto.main.temp),
                MaxTemp = Convert.ToInt32(dto.main.temp_max),
                MinTemp = Convert.ToInt32(dto.main.temp_min),
                Description = dto.weather.First().description
            };            
        }   
        catch (Exception ex)
        {
            throw new OpenWeatherParseException(json, ex);
        }
    }
}