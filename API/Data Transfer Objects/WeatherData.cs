using System.Text.Json;
using API.Architecture;
using API.Magazine;
using HtmlAgilityPack;

namespace API.DataTransferObjects
{
    public class WeatherData
    {
        MagazineContent _content;

        public readonly string TodayTitle;
        public readonly string TodayText;

        public readonly string TomorrowTitle;
        public readonly string TomorrowText;

        public readonly string OutlookTitle;
        public readonly string OutlookText;

        public readonly Dictionary<string, int> Temperatures = new();

        public readonly bool TempsValid = true;

        public WeatherData(MagazineContent content)
        {
            _content = content;

            try
            {
                Temperatures.Add("London", GetTempFromApiResponse(MagazineSectionType.WeatherTempLondon));
                Temperatures.Add("Cardiff", GetTempFromApiResponse(MagazineSectionType.WeatherTempCardiff));
                Temperatures.Add("Manchester", GetTempFromApiResponse(MagazineSectionType.WeatherTempManchester));
                Temperatures.Add("Edinburgh", GetTempFromApiResponse(MagazineSectionType.WeatherTempEdinburgh));
                Temperatures.Add("Belfast", GetTempFromApiResponse(MagazineSectionType.WeatherTempBelfast));
                Temperatures.Add("Lerwick", GetTempFromApiResponse(MagazineSectionType.WeatherTempLerwick));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.Source);
                TempsValid = false;
            }

            string html = content.UrlCache.First(l => l.Location == content.Sections.First(z => z.Name == MagazineSectionType.WeatherForecast).Feed).Content;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            TodayTitle = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[1]").InnerText;
            TomorrowTitle = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[2]").InnerText;
            OutlookTitle = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[3]").InnerText;

            TodayText = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[1]").NextSibling.InnerText;
            TomorrowText = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[2]").NextSibling.InnerText;
            OutlookText = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[3]").NextSibling.InnerText;
        }

        private int GetTempFromApiResponse(MagazineSectionType section)
        {
            string json = _content.UrlCache.First(l => l.Location == _content.Sections.First(z => z.Name == section).Feed).Content;
            OpenWeather dto = JsonSerializer.Deserialize<OpenWeather>(json);
            return Convert.ToInt32(dto.main.temp);
        }
    }
}