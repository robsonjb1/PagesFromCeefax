using System.Diagnostics;
using System.Text.Json;
using API.Architecture;
using API.DataTransferObjects;
using API.Magazine;
using HtmlAgilityPack;

namespace API.Services
{
    public interface IWeatherService
    {
        public WeatherData GetWeatherData();
    }

    public class WeatherService : IWeatherService
    {
        private readonly IMagazineContent _content;

        public WeatherService(IMagazineContent content)
        {
            _content = content;
        }

        public WeatherData GetWeatherData()
        {
            string html = _content.UrlCache.First(l => l.Location == _content.Sections.First(z => z.Name == MagazineSectionType.WeatherForecast).Feed).Content;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            WeatherData wd = new()
            {
                TodayTitle = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[1]").InnerText,
                TomorrowTitle = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[2]").InnerText,
                OutlookTitle = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[3]").InnerText,

                TodayText = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[1]").NextSibling.InnerText,
                TomorrowText = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[2]").NextSibling.InnerText,
                OutlookText = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[3]").NextSibling.InnerText,

                LastRefreshUTC = DateTime.UtcNow
            };

            try
            {
                wd.Temperatures.Add("London", GetTempFromApiResponse(MagazineSectionType.WeatherTempLondon));
                wd.Temperatures.Add("Cardiff", GetTempFromApiResponse(MagazineSectionType.WeatherTempCardiff));
                wd.Temperatures.Add("Manchester", GetTempFromApiResponse(MagazineSectionType.WeatherTempManchester));
                wd.Temperatures.Add("Edinburgh", GetTempFromApiResponse(MagazineSectionType.WeatherTempEdinburgh));
                wd.Temperatures.Add("Belfast", GetTempFromApiResponse(MagazineSectionType.WeatherTempBelfast));
                wd.Temperatures.Add("Lerwick", GetTempFromApiResponse(MagazineSectionType.WeatherTempLerwick));
                wd.Temperatures.Add("Truro", GetTempFromApiResponse(MagazineSectionType.WeatherTempTruro));
            }
            catch (OpenWeatherParseException ex)
            {
                if (Debugger.IsAttached)
                {
                    throw;
                }
                else
                {
                    // Silent fail. We will continue even if we have no spot temperatures.
                    // In this case, the weather map will not be shown.
                    Console.WriteLine($"{ex.Message} {ex.Source}");
                }
            }

            return wd;
        }

        private int GetTempFromApiResponse(MagazineSectionType section)
        {
            string json = String.Empty;
            try
            {
                json = _content.UrlCache.First(l => l.Location == _content.Sections.First(z => z.Name == section).Feed).Content;
                OpenWeather dto = JsonSerializer.Deserialize<OpenWeather>(json);
                return Convert.ToInt32(dto.main.temp);
            }
            catch (Exception ex)
            {
                throw new OpenWeatherParseException(json, ex);
            }
        }
    }
}