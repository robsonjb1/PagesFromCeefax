using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using API.Magazine;

namespace API.Architecture
{
    public static class Utility
    {
        public static char[] ConvertToCharArray(string line)
        {
            bool readingToken = false;
            string token = string.Empty;

            char[] input = line.ToCharArray();
            List<char> output = new();
            
            for(int i=0; i<line.Length; i++)
            {
                if(line[i] == '[')
                {
                    readingToken = true;
                    token = String.Empty;
                }
                else
                {
                    if(readingToken) 
                    {
                        token += line[i];
                    }
                }

                if(line[i] == ']')
                {
                    readingToken = false;
                    token = token[..^1];    // Strip off final ]
                    bool success = Enum.TryParse(token, out TeletextControl controlChar);
                    output.Add(success ? Convert.ToChar((int)controlChar) : Convert.ToChar(128));   // Use 128 to denote an invalid token, will be picked up by client app                    
                }
                else
                {
                    if(!readingToken)
                    {
                        if(line[i] == Convert.ToChar(8201))
                        {
                            output.Add(' ');
                            break;
                        }

                        switch(line[i])
                        {
                            case '£':
                                output.Add(Convert.ToChar((int)TeletextControl.Pound));
                                break;
                            case '’':
                            case '‘':
                                output.Add('\'');
                                break;
                            case '¬':
                                output.Add(Convert.ToChar((int)TeletextControl.Block));
                                break;
                            case '_':
                                output.Add(Convert.ToChar(96));
                                break;
                            default:
                                output.Add(line[i]);
                                break;
                        }           
                    }
                }
            }
            
            // Pad to 40 characters
            while(output.Count < 40)
            {
                output.Add(Convert.ToChar(0));
            }

            return output.ToArray();
        }


        // Line padding extension methods
        public static void PadLines(this StringBuilder sb, int totalLines)
        {
            if(totalLines > 0)
            {
                for(int i=0; i<totalLines; i++)  
                { sb.AppendLine(String.Empty); }
            }
        }

        public static string PadLeftWithTrunc(this string text, int maxChars)
        {
            if(text.Length >= maxChars)
            { return text[..maxChars]; }
            else
            { return text.PadLeft(maxChars); }
        }

        public static string PadRightWithTrunc(this string text, int maxChars)
        {
            if(text.Length >= maxChars)
            { return text[..maxChars]; }
            else
            { return text.PadRight(maxChars); }
        }

        public static string PadLeftWithTrunc(this int number, int maxChars)
        {
            string text = number.ToString();
            if(text.Length >= maxChars)
            { return text[..maxChars]; }
            else
            { return text.PadLeft(maxChars); }
        }

        public static void LineBreak(this StringBuilder sb, TeletextControl colour)
        {
            sb.AppendLine($"[{colour}]{new string('_', 39)}");
        }      

        // Output standard footer text
        public static void FooterText(this StringBuilder sb, CeefaxSection section, bool overrideDefault = false)
        {
            sb.Append($"[{section.PromoBackground}][{TeletextControl.NewBackground}][{section.PromoCol}]");
            if (overrideDefault && section.PromoFooter is not null)
            {
                sb.AppendLine(section.PromoFooter);
            }
            else
            {
                sb.AppendLine("More from CEEFAX in a moment >>>");
            }
        }

        // Show in UK time
        public static DateTime ConvertToUKTime(DateTime utcDateTime)
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, cstZone);
        }

        public static string GetDaySuffix(DateTime dt)
        {
            switch (dt.Day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        // Mixture of string parsing utilities and graphics character handling
        public static List<string> ParseParagraph(string content)
        {
            return ParseParagraph(content, 39, 39, true);
        }

        public static List<string> ParseParagraph(string content, int lineLength, int firstLineOverride, bool addFullStop)
        {
            List<string> rows = [];
            content = Utility.CleanHTML(content);

            // Ensure a final full stop or question mark
            if(addFullStop
                && !content.EndsWith('.') 
                && !content.EndsWith('?')
                && !content.EndsWith('”')
                && !content.EndsWith('"')
                && !content.EndsWith('\'')
                && !content.EndsWith('>')
                && content.Length > 0)
            {
                content += ".";
            }

            String[] words = content.Split(' ');
            string currentLine = "";
            int effectiveLineLength = firstLineOverride;
            
            foreach (string currentWord in words)
            {
                if ((currentLine.Length >= effectiveLineLength) ||
                    ((currentLine.Length + currentWord.Length) >= effectiveLineLength))
                {
                    if(effectiveLineLength != lineLength)
                    {
                        currentLine += currentLine.PadRightWithTrunc(firstLineOverride - currentLine.Length) + " x/y";
                    }

                    rows.Add(CleanHTML(currentLine));
                    currentLine = "";
                    effectiveLineLength = lineLength;
                }

                if (currentLine.Length > 0)
                {
                    currentLine += " " + currentWord;
                }
                else
                {
                    currentLine += currentWord;
                }
            }

            if (currentLine.Length > 0)
            {
                rows.Add(CleanHTML(currentLine));
            }

            return rows;
        }
                
        public static string CleanHTML(string html)
        {
            // Tags
            html = Regex.Replace(html, @"<[^>]+>", "").Trim().Replace(System.Environment.NewLine, "").Replace("&#039;", "'").Replace("&quot;", "'").Replace("\n", "");
            // Double spaces
            html = Regex.Replace(html, @"[ ]{2,}", @" ");

            // Line
            html = html.Replace("â€”", " - ");
            html = html.Replace('–', Convert.ToChar(TeletextControl.LongLine));

            // Pound
            //html = html.Replace("Â£", "£");
            html = html.Replace('£', Convert.ToChar(TeletextControl.Pound));
            
            // Euro
            html = html.Replace("â‚¬", "E");
            // Open/closing quotes
            html = html.Replace("“", "\"");
            html = html.Replace("”", "\"");
            
            html = html.Replace("â€œ", "'");
            html = html.Replace("â€™", "'");
            html = html.Replace("â€", "'");
            html = html.Replace("\\\"", "'");
            html = html.Replace("&#x27;", "'");
            html = html.Replace("[", "(");
            html = html.Replace("]", ")");
                       
            // Ampersand
            html = html.Replace("&amp;", "&");
        
            return html.Trim();
        }

    }
}