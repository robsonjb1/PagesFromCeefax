using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public class TeletextPageWeather(CeefaxContent cc, WeatherData wd)
{
    private readonly WeatherData _wd = wd;
    private readonly CeefaxContent _cc = cc;

    #region Public Methods
    public StringBuilder CreateWeatherMap()
    {
        StringBuilder sb = new();
        if(_wd.IsValid)             // Only construct the page if we have valid data
        {
            CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.WeatherUK);
        
            sb.AppendLine($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaWhite}]Data: BBC Weather Centre/Met Office");
            string map = Graphics.PromoMap.ToString();
            string summaryText = _wd.Forecasts[0].Body[.._wd.Forecasts[0].Body.IndexOf('.')];
            
            List<string> mapLines = Utility.ParseParagraph(summaryText, -1, true); // -1 special case to flow around the map

            TeletextControl summaryColour = TeletextControl.AlphaYellow;
            
            if(int.TryParse(_wd.Sunset[..2], out int sunsetHour) && int.TryParse(_wd.Sunrise[..2], out int sunriseHour))
            {
                if(DateTime.Now.Hour >= sunsetHour ||
                    DateTime.Now.Hour <= sunriseHour) // Night time
                {
                    summaryColour = TeletextControl.AlphaCyan;
                }
            }

            int j = 1;
            foreach (string line in mapLines)
            {
                string replacement = $"[{summaryColour}]{line.PadRightWithTrunc((j<8 ? 19 : 15))}";
                map = map.Replace("[LINE" + j.ToString() + "]", replacement);
                j++;
            }
            // Padding for any remaining lines
            for (int k = j; k <= 11; k++)
            {
                map = map.Replace("[LINE" + k.ToString() + "]", $"[{TeletextControl.AlphaWhite}]{new string(' ', (k<8 ? 19 : 15))}");
            }
          
            // Insert temperatures
            TeletextControl windCol = TeletextControl.AlphaWhite;
            if(_wd.Locations["Heathfield"].WindSpeed >= 30)
            {
                windCol = TeletextControl.AlphaRed;
            }
            TeletextControl pressureMovement = TeletextControl.RightArrow;
            if(_wd.Locations["Heathfield"].PressureRising)
            {
                pressureMovement = TeletextControl.UpArrow;
            }
            if(_wd.Locations["Heathfield"].PressureFalling)
            {
                pressureMovement = TeletextControl.Quarter;
            }
            
            map = map.Replace("[AA]", FormatWeatherString(_wd.Locations["Heathfield"].Temperature, _wd.Locations["Heathfield"].WindSpeed) + $"[{TeletextControl.GraphicsGreen}]")
                .Replace("[BB]", FormatWeatherString(_wd.Locations["Swansea"].Temperature, _wd.Locations["Swansea"].WindSpeed) + $"[{TeletextControl.GraphicsGreen}]")
                .Replace("[CC]", FormatWeatherString(_wd.Locations["Manchester"].Temperature, _wd.Locations["Manchester"].WindSpeed) + $"[{TeletextControl.GraphicsGreen}]")
                .Replace("[DD]", FormatWeatherString(_wd.Locations["Edinburgh"].Temperature, _wd.Locations["Edinburgh"].WindSpeed) + $"[{TeletextControl.GraphicsGreen}]")
                .Replace("[EE]", FormatWeatherString(_wd.Locations["Belfast"].Temperature, _wd.Locations["Belfast"].WindSpeed) + $"[{TeletextControl.GraphicsGreen}]")
                .Replace("[FF]", FormatWeatherString(_wd.Locations["Inverness"].Temperature, _wd.Locations["Inverness"].WindSpeed) + $"[{TeletextControl.GraphicsGreen}]")
                .Replace("[GG]", FormatWeatherString(_wd.Locations["Penzance"].Temperature, _wd.Locations["Penzance"].WindSpeed) + $"[{TeletextControl.GraphicsGreen}]")
                .Replace("[HH]", FormatWeatherString(_wd.Locations["Cambridge"].Temperature, _wd.Locations["Cambridge"].WindSpeed) + $"[{TeletextControl.GraphicsGreen}]")
                .Replace("[II]", FormatWeatherString(_wd.Locations["Birmingham"].Temperature, _wd.Locations["Birmingham"].WindSpeed) + $"[{TeletextControl.GraphicsGreen}]")
                .Replace("[TTT]", Utility.ConvertToUKTime(_wd.LastRefreshUTC).ToString("HH:mm"))
                .Replace("[SU1]", _wd.Sunrise.PadRightWithTrunc(5))
                .Replace("[SU2]", _wd.Sunset.PadRightWithTrunc(5))
                .Replace("[TE1]", $"[{TemperatureColour(_wd.MinTemp)}]{_wd.MinTempLabel.PadRightWithTrunc(5)}")
                .Replace("[TE2]", $"[{TemperatureColour(_wd.MaxTemp)}]{_wd.MaxTempLabel.PadRightWithTrunc(5)}")
                .Replace("[PR1]", $"[{pressureMovement}]{_wd.Locations["Heathfield"].Pressure.PadLeftWithTrunc(4)}")
                .Replace("[PR2]", $"[{windCol}]{_wd.Locations["Heathfield"].WindDirection.PadRightWithTrunc(12)}");                
                
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
            CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.WeatherUK);
            
            int forecastNo = 0;
            switch (page)
            {
                case CeefaxSectionType.WeatherForecast1:
                    forecastNo = 1;
                    break;
                case CeefaxSectionType.WeatherForecast2:
                    forecastNo = 2;
                    break;
                default:
                    break;
            }

            string sectionTitle = _wd.Forecasts[forecastNo].Title.ToUpper().PadRightWithTrunc(35);
            string sectionText = _wd.Forecasts[forecastNo].Body;

            sb.Append(Graphics.HeaderWeather);
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]{sectionTitle}[{TeletextControl.AlphaWhite}]{forecastNo + 1}/3");

            // Break body text up into paragraphs
            List<string> bodyLines = [];
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

            sb.FooterText(section, forecastNo == 2);
        }
        return sb;
    }
    #endregion

    #region Private Methods
    private static string FormatWeatherString(int temperature, int windSpeed)
    {
        TeletextControl startCol = TemperatureColour(temperature);

        if(windSpeed >= 30)
        {
            startCol = TeletextControl.AlphaRed;
        }

        string str = temperature.ToString();
        str = $"[{startCol}]{str.PadLeftWithTrunc(2)}";
        
        return str;
    }

    private static TeletextControl TemperatureColour(int temperature)
    {
        TeletextControl col = TeletextControl.AlphaWhite;

        if(temperature >= 20)
        {
            col = TeletextControl.AlphaYellow;
        }
        if(temperature <= 3)
        {
            col = TeletextControl.AlphaCyan;
        }
        if(temperature >= 30)
        {
            col = TeletextControl.AlphaRed;
        }

        return col;
    }
    #endregion
}