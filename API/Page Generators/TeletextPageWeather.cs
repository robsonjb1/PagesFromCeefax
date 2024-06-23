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
        
            sb.AppendLineColour("&nbsp;&nbsp;Data: BBC Weather Centre/Met Office&nbsp;&nbsp;", Mode7Colour.White, Mode7Colour.Blue);
            string map = Graphics.PromoMap.ToString();
            string summaryText = _wd.Forecasts[0].Body;
            if (summaryText.Contains('.'))
            {
                summaryText = summaryText[..(summaryText.IndexOf('.') + 1)];
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
                string replacement = $"<span class=\"ink{(int)summaryColour} indent\">" + line.PadHtmlLeft(19) + "</span>";
                map = map.Replace("[LINE" + j.ToString() + "]", replacement);
                j++;
            }
            // Padding for any remaining lines
            for (int k = j; k <= 7; k++)
            {
                map = map.Replace("[LINE" + k.ToString() + "]", $"<span class=\"ink{(int)Mode7Colour.White} indent\">{"".PadHtmlLeft(19)}</span>");
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
            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">{sectionTitle}</span><span class=\"ink{(int)Mode7Colour.White}\">{forecastNo+2-offset}/5</p>");

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
                sb.AppendLineColour(line, firstLine ? Mode7Colour.White : Mode7Colour.Cyan);
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
                sb.AppendLineColour("Data: BBC Weather Centre/Met Office", Mode7Colour.Green);
                sb.AppendLine("<br>");
            }

            sb.FooterText(section);
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
            sb.AppendLineColour($"WORLD CITIES (US/EUROPE/OTHER){Utility.LineColourFragment("5/5".PadHtmlRight(9), Mode7Colour.White)}", Mode7Colour.Yellow);
            sb.AppendLineColour($"max min conditions".PadHtmlRight(33), Mode7Colour.Green);
            
            OutputWorldCity(sb, "San Francisco", Mode7Colour.White);
            OutputWorldCity(sb, "New York", Mode7Colour.Cyan);         
            
            sb.LineBreak(Mode7Colour.Blue);           
            OutputWorldCity(sb, "London", Mode7Colour.White);
            OutputWorldCity(sb, "Edinburgh", Mode7Colour.Cyan);
            OutputWorldCity(sb, "Paris", Mode7Colour.White);
            OutputWorldCity(sb, "Madrid", Mode7Colour.Cyan);
            OutputWorldCity(sb, "Munich", Mode7Colour.White);
            OutputWorldCity(sb, "Krakow", Mode7Colour.Cyan);
            
            sb.LineBreak(Mode7Colour.Blue);           
            OutputWorldCity(sb, "Cape Town", Mode7Colour.White);
            OutputWorldCity(sb, "Chennai", Mode7Colour.Cyan);
            OutputWorldCity(sb, "Singapore", Mode7Colour.White);
            OutputWorldCity(sb, "Tokyo", Mode7Colour.Cyan);
            OutputWorldCity(sb, "Sydney", Mode7Colour.White);
            OutputWorldCity(sb, "Wellington", Mode7Colour.Cyan);
            
            sb.AppendLine("<br>");
            sb.FooterText(section);
        }
        return sb;
    }

    #endregion

    #region Private Methods
    private void OutputWorldCity(StringBuilder sb, string city, Mode7Colour colour)
    {
        // City name
        string partCity = city.PadHtmlLeft(13);
        
        // City max/min temperatures
        string partTemps = FormatWeatherString(_wd.Temperatures[city].MaxTemp, colour) +
            FormatWeatherString(_wd.Temperatures[city].MinTemp, colour);   
        
        // City conditions
        string partConditions = _wd.Temperatures[city].Description;
        partConditions = partConditions[..1].ToUpper() + partConditions[1..];                   // Upper case first letter
        partConditions = partConditions.Length > 16 ? partConditions[..16] : partConditions;    // Ensure 16 characters max
        
        sb.AppendLineColour($"{partCity} {partTemps} {partConditions}", colour);
    }
    private static string FormatWeatherString(int temperature, Mode7Colour colour = Mode7Colour.White)
    {
        string str = temperature.ToString();
        if (temperature >= 0)
        {
            str = Utility.LineColourFragment("&nbsp;" + ((str.Length == 1) ? "&nbsp;" : "") + str + "&nbsp;", colour);
        }
        else
        {
            str = Utility.LineColourFragment("&nbsp;" + ((str.Length == 2) ? "&nbsp;" : "") + str + "&nbsp;", colour);
        }

        return str;
    }
    #endregion
}