using System.Text;
using API.Architecture;
using API.DataTransferObjects;
using API.Magazine;

namespace API.PageGenerators
{
    public interface ITeletextPageWeather
    {
        public StringBuilder CreateWeatherMap(WeatherData weather);

        public StringBuilder CreateWeather(int sectionPage, string sectionTitle, string sectionText);
    }

    public class TeletextPageWeather : ITeletextPageWeather
    {
        MagazineContent _mc;
     
        public TeletextPageWeather(MagazineContent mc)
        {
            _mc = mc;
        }

        private string FormatWeatherString(int temperature)
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

        public StringBuilder CreateWeatherMap(WeatherData weather)
        {
            StringBuilder sb = new();
            sb.Append($"<p><span class=\"paper{(int)Mode7Colour.Blue} ink{(int)Mode7Colour.White}\">&nbsp;&nbsp;Data: BBC Weather Centre/Met Office&nbsp;&nbsp;</span></p>");

            string map = Graphics.PromoMap.ToString();
            string summaryText = weather.TodayText;
            if (summaryText.Contains('.'))
            {
                summaryText = summaryText.Substring(0, summaryText.IndexOf(".") + 1);
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
            for (int k=j; k <= 7; k++)
            {
                map = map.Replace("[LINE" + k.ToString() + "]", "<span class=\"ink7 indent\">" + String.Join("", Enumerable.Repeat("&nbsp;", 18)) + "</span>");
            }


            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            DateTime ukTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cstZone);

            map = map.Replace("[AA]", FormatWeatherString(weather.Temperatures["London"]))
                .Replace("[BB]", FormatWeatherString(weather.Temperatures["Cardiff"]))
                .Replace("[CC]", FormatWeatherString(weather.Temperatures["Manchester"]))
                .Replace("[DD]", FormatWeatherString(weather.Temperatures["Edinburgh"]))
                .Replace("[EE]", FormatWeatherString(weather.Temperatures["Belfast"]))
                .Replace("[FF]", FormatWeatherString(weather.Temperatures["Lerwick"]))
                .Replace("[TTT]", ukTime.ToString("HH:mm"));

            sb.Append(map);

            sb.Append($"<p><span class=\"paper{(int)Mode7Colour.Blue} ink{(int)Mode7Colour.Yellow}\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");

            return sb;
        }

        public StringBuilder CreateWeather(int sectionPage, string sectionTitle, string sectionText)
        {
            StringBuilder sb = new StringBuilder();
            sectionTitle = sectionTitle.ToUpper() + string.Join("", Enumerable.Repeat("&nbsp;", 36 - sectionTitle.Length));

            sb.Append(Graphics.HeaderWeather);
            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">{sectionTitle}</span><span class=\"ink{(int)Mode7Colour.White}\">{sectionPage}/4</p>");
            
            List<string> bodyLines = new List<string>();

            // Break body text up into paragraphs
            string content = $"<p>{sectionText}</p>";
            bool pageLengthExceeded = false;
            content = content.Replace(".", ".</p><p>");

            while (content.Contains("<p>") && !pageLengthExceeded)
            {
                content = content.Substring(content.IndexOf("<p>") + 3);

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
                if(line == String.Empty)
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

    }
}