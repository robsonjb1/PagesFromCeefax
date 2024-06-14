using System.IO.Compression;
using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public interface ITeletextPageWeather
{
    public StringBuilder CreateWeatherMap();
    public StringBuilder CreateWeatherPage(CeefaxSectionType page);
    public StringBuilder CreateWeatherWorld();
}

public class TeletextPageWeather : ITeletextPageWeather
{
    private readonly IWeatherData _wd;
    private readonly ICeefaxContent _cc;

    public TeletextPageWeather(ICeefaxContent cc, IWeatherData wd)
    {
        _cc = cc;
        _wd = wd;
    }

    #region Public Methods
    public StringBuilder CreateWeatherMap()
    {
        StringBuilder sb = new();
        if(_wd.IsValid)             // Only construct the page if we have valid data
        {
            CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.Weather);
        
            sb.Append($"<p><span class=\"paper{(int)Mode7Colour.Blue} ink{(int)Mode7Colour.White}\">&nbsp;&nbsp;Data: BBC Weather Centre/Met Office&nbsp;&nbsp;</span></p>");

            string map = Graphics.PromoMap.ToString();
            string summaryText = _wd.Forecasts[0].Body;
            if (summaryText.Contains('.'))
            {
                summaryText = summaryText[..(summaryText.IndexOf(".") + 1)];
            }

            List<string> mapLines = Utility.ParseParagraph(summaryText, 18, 18, true);

            Mode7Colour summaryColour = Mode7Colour.Yellow;
            if (DateTime.Now.Hour >= 18 || DateTime.Now.Hour <= 6)
            {
                summaryColour = Mode7Colour.Cyan;
            }

            int j = 1;
            foreach (string line in mapLines)
            {
                string replacement = $"<span class=\"ink{(int)summaryColour} indent\">" + line.PadHtmlRight(18) + "</span>";
                map = map.Replace("[LINE" + j.ToString() + "]", replacement);
                j++;
            }
            // Padding for any remaining lines
            for (int k = j; k <= 7; k++)
            {
                map = map.Replace("[LINE" + k.ToString() + "]", $"<span class=\"ink{(int)Mode7Colour.White} indent\">{"".PadHtmlLeft(18)}</span>");
            }

            // Insert temperatures
            map = map.Replace("[AA]", FormatWeatherString(_wd.Temperatures["London"].CurrentTemp))
                .Replace("[BB]", FormatWeatherString(_wd.Temperatures["Cardiff"].CurrentTemp))
                .Replace("[CC]", FormatWeatherString(_wd.Temperatures["Manchester"].CurrentTemp))
                .Replace("[DD]", FormatWeatherString(_wd.Temperatures["Edinburgh"].CurrentTemp))
                .Replace("[EE]", FormatWeatherString(_wd.Temperatures["Belfast"].CurrentTemp))
                .Replace("[FF]", FormatWeatherString(_wd.Temperatures["Lerwick"].CurrentTemp))
                .Replace("[GG]", FormatWeatherString(_wd.Temperatures["Truro"].CurrentTemp))
                .Replace("[TTT]", Utility.ConvertToUKTime(_wd.LastRefreshUTC).ToString("HH:mm"));

            sb.Append(map);

            Utility.FooterText(sb, section);
        }
        return sb;
    }

    public StringBuilder CreateWeatherPage(CeefaxSectionType page)
    {
        StringBuilder sb = new();
        if(_wd.IsValid)             // Only construct the page if we have valid data
        {
            CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.Weather);
            
            int forecastNo = 0;
            int offset = _wd.Forecasts[3].Body != null ? 1 : 0; // Show the last three forecasts
            switch (page)
            {
                case CeefaxSectionType.WeatherForecast1:
                    forecastNo = 0 + offset;
                    break;
                case CeefaxSectionType.WeatherForecast2:
                    forecastNo = 1 + offset;
                    break;
                case CeefaxSectionType.WeatherForecast3:
                    forecastNo = 2 + offset;
                    break;
                default:
                    break;
            }

            string sectionTitle = _wd.Forecasts[forecastNo].Title.ToUpper().PadHtmlLeft(36);
            string sectionText = _wd.Forecasts[forecastNo].Body;

            sb.Append(Graphics.HeaderWeather);
            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">{sectionTitle}</span><span class=\"ink{(int)Mode7Colour.White}\">{forecastNo+1}/5</p>");

            // Break body text up into paragraphs
            List<string> bodyLines = new();
            foreach(string line in sectionText.Split(". "))
            {
                List<string> newChunk = Utility.ParseParagraph(line);

                if (newChunk.Count > 0)
                {
                    if (bodyLines.Count + newChunk.Count > 16)
                    {
                        break;
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

            Utility.PadLines(sb, lastLine - bodyLines.Count);

            if (lastLine == 15)
            {
                sb.AppendLine("<br>");
                sb.Append($"<p><span class=\"ink{(int)Mode7Colour.Green}\">Data: BBC Weather Centre/Met Office</span></p>");
                sb.AppendLine("<br>");
            }

            Utility.FooterText(sb, section);
        }
        return sb;
    }

    public StringBuilder CreateWeatherWorld()
    {
        StringBuilder sb = new();
        if(_wd.IsValid)             // Only construct the page if we have valid data
        {
            CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.Weather);

            sb.Append(Graphics.HeaderWeather);
            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">WORLD CITIES{string.Join("", Enumerable.Repeat("&nbsp;", 23))}</span>");
            sb.Append("<span class=\"ink{(int)Mode7Colour.White}\">5/5</p>");
            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">(EUROPE)</span>");
            sb.Append($"<span class=\"ink{(int)Mode7Colour.Green} indent\">{"max min conditions".PadHtmlRight(24)}</span></p>");
            
            OutputWorldCity(sb, "London", Mode7Colour.White);
            OutputWorldCity(sb, "Edinburgh", Mode7Colour.Cyan);
            OutputWorldCity(sb, "Paris", Mode7Colour.White);
            OutputWorldCity(sb, "Madrid", Mode7Colour.Cyan);
            OutputWorldCity(sb, "Munich", Mode7Colour.White);
            OutputWorldCity(sb, "Krakow", Mode7Colour.Cyan);

            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">(US)</span></p>");
            OutputWorldCity(sb, "San Francisco", Mode7Colour.White);
            OutputWorldCity(sb, "New York", Mode7Colour.Cyan);          
            
            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">(WORLD)</span></p>");
            OutputWorldCity(sb, "Cape Town", Mode7Colour.White);
            OutputWorldCity(sb, "Chennai", Mode7Colour.Cyan);
            OutputWorldCity(sb, "Singapore", Mode7Colour.White);
            OutputWorldCity(sb, "Tokyo", Mode7Colour.Cyan);
            OutputWorldCity(sb, "Sydney", Mode7Colour.White);
            OutputWorldCity(sb, "Wellington", Mode7Colour.Cyan);
            
            sb.AppendLine("<br>");
            Utility.FooterText(sb, section);
        }
        return sb;
    }

    #endregion

    #region Private Methods
    private void OutputWorldCity(StringBuilder sb, string city, Mode7Colour colour)
    {
        // City name
        sb.AppendLine($"<p><span class=\"ink{(int)colour} indent\">{city.PadHtmlLeft(13)}");
        
        // Max/min temperatures
        sb.Append($"{FormatWeatherString(_wd.Temperatures[city].MaxTemp, colour)}");
        sb.AppendLine($"{FormatWeatherString(_wd.Temperatures[city].MinTemp, colour)}");
        
        // Conditions
        string description = _wd.Temperatures[city].Description;
        description = description.Substring(0, 1).ToUpper() + description.Substring(1);         // Upper case first letter
        description = description.Length > 16 ? description.Substring(0, 16) : description;     // Ensure 16 characters max
        
        sb.AppendLine(description);
        sb.AppendLine("</span></p>");
    }
    private string FormatWeatherString(int temperature, Mode7Colour colour = Mode7Colour.White)
    {
        string str = temperature.ToString();
        if (temperature >= 0)
        {
            str = $"<span class=\"ink{(int)colour}\">&nbsp;" + ((str.Length == 1) ? "&nbsp;" : "") + str + "&nbsp;</span>";
        }
        else
        {
            str = $"<span class=\"ink{(int)colour}\">" + ((str.Length == 2) ? "&nbsp;" : "") + str + "&nbsp;</span>";
        }

        return str;
    }
    #endregion
}