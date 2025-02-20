﻿using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public interface ITeletextPageWeather
{
    public StringBuilder CreateWeatherMap();
    public StringBuilder CreateWeatherPage(CeefaxSectionType page);
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
        
            sb.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}]Data: BBC Weather Centre/Met Office");
            string map = Graphics.PromoMap.ToString();
            string summaryText = _wd.Forecasts[0].Body;
            if (summaryText.Contains('.'))
            {
                summaryText = summaryText[..(summaryText.IndexOf('.') + 1)];
            }

            List<string> mapLines = Utility.ParseParagraph(summaryText, 19, 19, true);

            TeletextControl summaryColour = TeletextControl.AlphaYellow;
            if (Utility.ConvertToUKTime(DateTime.UtcNow).Hour >= 18 || Utility.ConvertToUKTime(DateTime.UtcNow).Hour <= 6)
            {
                summaryColour = TeletextControl.AlphaCyan;
            }

            int j = 1;
            foreach (string line in mapLines)
            {
                string replacement = $"[{summaryColour}]{line.PadRightWithTrunc(19)}";
                map = map.Replace("[LINE" + j.ToString() + "]", replacement);
                j++;
            }
            // Padding for any remaining lines
            for (int k = j; k <= 7; k++)
            {
                map = map.Replace("[LINE" + k.ToString() + "]", $"[{TeletextControl.AlphaWhite}]{new string(' ', 19)}");
            }

            // Insert temperatures
            map = map.Replace("[AA]", FormatWeatherString(_wd.Temperatures["London"].CurrentTemp, TeletextControl.AlphaWhite, TeletextControl.GraphicsGreen))
                .Replace("[BB]", FormatWeatherString(_wd.Temperatures["Cardiff"].CurrentTemp, TeletextControl.AlphaWhite, TeletextControl.GraphicsGreen))
                .Replace("[CC]", FormatWeatherString(_wd.Temperatures["Manchester"].CurrentTemp, TeletextControl.AlphaWhite, TeletextControl.GraphicsGreen))
                .Replace("[DD]", FormatWeatherString(_wd.Temperatures["Edinburgh"].CurrentTemp, TeletextControl.AlphaWhite, TeletextControl.GraphicsGreen))
                .Replace("[EE]", FormatWeatherString(_wd.Temperatures["Belfast"].CurrentTemp, TeletextControl.AlphaWhite, TeletextControl.GraphicsGreen))
                .Replace("[FF]", FormatWeatherString(_wd.Temperatures["Lerwick"].CurrentTemp, TeletextControl.AlphaWhite, TeletextControl.GraphicsGreen))
                .Replace("[GG]", FormatWeatherString(_wd.Temperatures["Truro"].CurrentTemp, TeletextControl.AlphaWhite, TeletextControl.GraphicsGreen))
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

            string sectionTitle = _wd.Forecasts[forecastNo].Title.ToUpper().PadRightWithTrunc(35);
            string sectionText = _wd.Forecasts[forecastNo].Body;

            sb.Append(Graphics.HeaderWeather);
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]{sectionTitle}[{TeletextControl.AlphaWhite}]{forecastNo+2-offset}/4");

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
                            bodyLines.Add(String.Empty);
                        }
                        bodyLines.AddRange(newChunk);
                    }
                }
            }

            bool firstLine = true;
            foreach (string line in bodyLines)
            {
                sb.AppendLine($"[{(firstLine ? TeletextControl.AlphaWhite : TeletextControl.AlphaCyan)}]{line}");

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
                sb.AppendLine(String.Empty);
                sb.AppendLine($"[{TeletextControl.AlphaGreen}]Data: BBC Weather Centre/Met Office");
                sb.AppendLine(String.Empty);
            }

            sb.FooterText(section);
        }
        return sb;
    }
    #endregion

    #region Private Methods
    private static string FormatWeatherString(int temperature, TeletextControl startCol = TeletextControl.AlphaWhite, TeletextControl endCol = TeletextControl.AlphaWhite)
    {
        string str = temperature.ToString();
        str = $"[{startCol}]{((str.Length == 1) ? " " : "")}{str}[{endCol}]";
        
        return str;
    }
    #endregion
}