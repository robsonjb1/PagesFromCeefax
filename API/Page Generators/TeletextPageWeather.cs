using System.Diagnostics;
using System.Text;
using System.Text.Json;
using API.Architecture;
using API.DataTransferObjects;
using API.Magazine;
using API.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Collections.Specialized.BitVector32;

namespace API.PageGenerators
{
    public interface ITeletextPageWeather
    {
        public StringBuilder CreateWeatherMap();
        public StringBuilder CreateWeatherPage(WeatherPage page);
    }

    public class TeletextPageWeather : ITeletextPageWeather
    {
        private readonly WeatherData _wd;
        private readonly IMagazineContent _mc;

        public TeletextPageWeather(IMagazineContent mc)
        {
            _mc = mc;

            string html = _mc.UrlCache.First(l => l.Location == _mc.Sections.First(z => z.Name == MagazineSectionType.WeatherForecast).Feed).Content;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            _wd = new()
            {
                TodayTitle = doc.DocumentNode.SelectSingleNode("(//b[@class='ssrcss-1xjjfut-BoldText e5tfeyi3'])[1]").InnerText,
                TomorrowTitle = doc.DocumentNode.SelectSingleNode("(//b[@class='ssrcss-1xjjfut-BoldText e5tfeyi3'])[2]").InnerText,
                OutlookTitle = doc.DocumentNode.SelectSingleNode("(//b[@class='ssrcss-1xjjfut-BoldText e5tfeyi3'])[3]").InnerText,

                TodayText = doc.DocumentNode.SelectSingleNode("(//b[@class='ssrcss-1xjjfut-BoldText e5tfeyi3'])[1]/parent::p/following-sibling::p").InnerText,
                TomorrowText = doc.DocumentNode.SelectSingleNode("(//b[@class='ssrcss-1xjjfut-BoldText e5tfeyi3'])[2]/parent::p/following-sibling::p").InnerText,
                OutlookText = doc.DocumentNode.SelectSingleNode("(//b[@class='ssrcss-1xjjfut-BoldText e5tfeyi3'])[3]/parent::p/following-sibling::p").InnerText,

                LastRefreshUTC = DateTime.UtcNow
            };

            try
            {
                _wd.Temperatures.Add("London", GetTempFromApiResponse(MagazineSectionType.WeatherTempLondon));
                _wd.Temperatures.Add("Cardiff", GetTempFromApiResponse(MagazineSectionType.WeatherTempCardiff));
                _wd.Temperatures.Add("Manchester", GetTempFromApiResponse(MagazineSectionType.WeatherTempManchester));
                _wd.Temperatures.Add("Edinburgh", GetTempFromApiResponse(MagazineSectionType.WeatherTempEdinburgh));
                _wd.Temperatures.Add("Belfast", GetTempFromApiResponse(MagazineSectionType.WeatherTempBelfast));
                _wd.Temperatures.Add("Lerwick", GetTempFromApiResponse(MagazineSectionType.WeatherTempLerwick));
                _wd.Temperatures.Add("Truro", GetTempFromApiResponse(MagazineSectionType.WeatherTempTruro));
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
        }

        #region Public Methods
        public StringBuilder CreateWeatherMap()
        {
            StringBuilder sb = new();

            // Only create the map if we have temperatures for all locations
            if (_wd.Temperatures.Count > 0)
            {
                sb.Append($"<p><span class=\"paper{(int)Mode7Colour.Blue} ink{(int)Mode7Colour.White}\">&nbsp;&nbsp;Data: BBC Weather Centre/Met Office&nbsp;&nbsp;</span></p>");

                string map = Graphics.PromoMap.ToString();
                string summaryText = _wd.TodayText;
                if (summaryText.Contains('.'))
                {
                    summaryText = summaryText[..(summaryText.IndexOf(".") + 1)];
                }

                List<string> mapLines = Utility.ParseParagraph(summaryText, 18, 18);

                Mode7Colour summaryColour = Mode7Colour.Yellow;
                if (DateTime.Now.Hour >= 18 || DateTime.Now.Hour <= 6)
                {
                    summaryColour = Mode7Colour.Cyan;
                }

                int j = 1;
                foreach (string line in mapLines)
                {
                    string replacement = $"<span class=\"ink{(int)summaryColour} indent\">" + line.PadRight(18).Replace(" ", "&nbsp;") + "</span>";
                    map = map.Replace("[LINE" + j.ToString() + "]", replacement);
                    j++;
                }
                // Padding for any remaining lines
                for (int k = j; k <= 7; k++)
                {
                    map = map.Replace("[LINE" + k.ToString() + "]", $"<span class=\"ink{(int)Mode7Colour.White} indent\">" + String.Join("", Enumerable.Repeat("&nbsp;", 18)) + "</span>");
                }

                // Insert temperatures
                map = map.Replace("[AA]", FormatWeatherString(_wd.Temperatures["London"]))
                    .Replace("[BB]", FormatWeatherString(_wd.Temperatures["Cardiff"]))
                    .Replace("[CC]", FormatWeatherString(_wd.Temperatures["Manchester"]))
                    .Replace("[DD]", FormatWeatherString(_wd.Temperatures["Edinburgh"]))
                    .Replace("[EE]", FormatWeatherString(_wd.Temperatures["Belfast"]))
                    .Replace("[FF]", FormatWeatherString(_wd.Temperatures["Lerwick"]))
                    .Replace("[GG]", FormatWeatherString(_wd.Temperatures["Truro"]))
                    .Replace("[TTT]", Utility.ConvertToUKTime(_wd.LastRefreshUTC).ToString("HH:mm"));

                sb.Append(map);

                sb.Append($"<p><span class=\"paper{(int)Mode7Colour.Blue} ink{(int)Mode7Colour.Yellow}\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");
            }
            return sb;
        }

        public StringBuilder CreateWeatherPage(WeatherPage page)
        {
            StringBuilder sb = new();

            string sectionTitle = String.Empty;
            string sectionText = String.Empty;
            int sectionPage = 0;

            switch (page)
            {
                case WeatherPage.Today:
                    sectionTitle = _wd.TodayTitle;
                    sectionText = _wd.TodayText;
                    sectionPage = 2;
                    break;
                case WeatherPage.Tomorrow:
                    sectionTitle = _wd.TomorrowTitle;
                    sectionText = _wd.TomorrowText;
                    sectionPage = 3;
                    break;
                case WeatherPage.Outlook:
                    sectionTitle = _wd.OutlookTitle;
                    sectionText = _wd.OutlookText;
                    sectionPage = 4;
                    break;
                default:
                    break;
            }

            sectionTitle = sectionTitle.ToUpper() + string.Join("", Enumerable.Repeat("&nbsp;", 36 - sectionTitle.Length));

            sb.Append(Graphics.HeaderWeather);
            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">{sectionTitle}</span><span class=\"ink{(int)Mode7Colour.White}\">{sectionPage}/4</p>");

            List<string> bodyLines = new();

            // Break body text up into paragraphs
            string content = $"<p>{sectionText}</p>";
            bool pageLengthExceeded = false;
            content = content.Replace(".", ".</p><p>");

            while (content.Contains("<p>") && !pageLengthExceeded)
            {
                content = content[(content.IndexOf("<p>") + 3)..];

                List<string> newChunk = Utility.ParseParagraph(content);

                if (newChunk.Count > 0)
                {
                    if (bodyLines.Count + newChunk.Count > 16)
                    {
                        pageLengthExceeded = true;
                    }
                    else
                    {
                        if (bodyLines.Count > 0)
                        {
                            bodyLines.Add("");
                        }
                        bodyLines.AddRange(newChunk);
                    }
                }
            }

            bool firstLine = true;
            foreach (string line in bodyLines)
            {
                sb.AppendLine($"<p><span class=\"ink{(firstLine ? (int)Mode7Colour.White : (int)Mode7Colour.Cyan)} indent\">{line}</span></p>");
                if (line == String.Empty)
                {
                    firstLine = false;
                }
            }

            // Optionally display met office notice
            int lastLine = 18;
            if (bodyLines.Count <= 15)
            {
                lastLine = 15;
            }

            for (int j = bodyLines.Count; j < lastLine; j++)
            {
                sb.AppendLine("<br>");
            }

            if (lastLine == 15)
            {
                sb.AppendLine("<br>");
                sb.Append($"<p><span class=\"ink{(int)Mode7Colour.Green}\">Data: BBC Weather Centre/Met Office</span></p>");
                sb.AppendLine("<br>");
            }

            sb.Append($"<p><span class=\"paper{(int)Mode7Colour.Blue} ink{(int)Mode7Colour.Yellow}\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");


            return sb;
        }
        #endregion

        #region Private Methods
        private static string FormatWeatherString(int temperature)
        {
            string str = temperature.ToString();
            if (temperature >= 0)
            {
                str = $"<span class=\"ink{(int)Mode7Colour.White}\">&nbsp;" + ((str.Length == 1) ? "&nbsp;" : "") + str + "&nbsp;</span>";
            }
            else
            {
                str = $"<span class=\"ink{(int)Mode7Colour.White}\">" + ((str.Length == 2) ? "&nbsp;" : "") + str + "&nbsp;</span>";
            }

            return str;
        }

        private int GetTempFromApiResponse(MagazineSectionType section)
        {
            string json = String.Empty;
            try
            {
                json = _mc.UrlCache.First(l => l.Location == _mc.Sections.First(z => z.Name == section).Feed).Content;
                OpenWeather dto = JsonSerializer.Deserialize<OpenWeather>(json);
                return Convert.ToInt32(dto.main.temp);
            }
            catch (Exception ex)
            {
                throw new OpenWeatherParseException(json, ex);
            }
        }
        #endregion
    }
}