using System.Text;
using System.Text.RegularExpressions;
using API.Magazine;

namespace API.Architecture
{
    public static class Utility
    {
        // Line padding
        public static void PadLines(StringBuilder sb, int totalLines)
        {
            for(int i=0; i<totalLines; i++)  
            {
                sb.Append("<br>");  
            }
        }

        // Output standard footer text
        public static void FooterText(StringBuilder sb, CeefaxSection section, bool overrideDefault = false)
        {
            if (overrideDefault && section.PromoFooter is not null)
            {
                string promoFooter = section.PromoFooter + string.Join("", Enumerable.Repeat("&nbsp;", 37 - section.PromoFooter.Length));
                sb.AppendLine($"<p><span class=\"paper{(int)section.PromoPaper!} ink{(int)section.PromoInk!}\">&nbsp;&nbsp;{promoFooter}</span></p>");
            }
            else
            {
                sb.AppendLine($"<p><span class=\"paper{(int)section.PromoPaper!} ink{(int)section.PromoInk!}\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");
            }
        }

        // Show in UK time
        public static DateTime ConvertToUKTime(DateTime utcDateTime)
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, cstZone);
        }

        // Mixture of string parsing utilities and graphics character handling
        public static List<string> ParseParagraph(string content)
        {
            return ParseParagraph(content, 39, 39, true);
        }

        public static List<string> ParseParagraph(string content, int lineLength, int firstLineOverride, bool addFullStop)
        {
            List<string> rows = new();
            //content += "</p>";  // In case there isn't already an ending </p>
            content = Utility.CleanHTML(content);//[..content.IndexOf("</p>")]);

            // Ensure a final full stop or question mark
            if(addFullStop
                && !content.EndsWith(".") 
                && !content.EndsWith("?")
                && !content.EndsWith("”")
                && !content.EndsWith("\"")
                && !content.EndsWith("'")
                && !content.EndsWith(">")
                && content.Length > 0)
            {
                content = content + ".";
            }

            String[] words = content.Split(' ');
            string currentLine = "";
            bool invalidText = false;
            int effectiveLineLength = firstLineOverride;
            
            foreach (string currentWord in words)
            {
           //     if (currentWord.ToUpper() == "JAVASCRIPT" || currentWord.ToUpper() == "(CSS)")
           //     {
           //         invalidText = true;
           //     }

                if (!invalidText)
                {
                    if ((currentLine.Length >= effectiveLineLength) ||
                        ((currentLine.Length + currentWord.Length) >= effectiveLineLength))
                    {
                        if(effectiveLineLength != lineLength)
                        {
                            currentLine += string.Join("", Enumerable.Repeat("&nbsp;", firstLineOverride - currentLine.Length)) + " x/y";
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
            }

            if (currentLine.Length > 0)
            {
                rows.Add(CleanHTML(currentLine));
            }

            //if (invalidText || currentLine.EndsWith(":") || currentLine.EndsWith("&hellip;"))
            //{
            //    return new List<string>();
            //}
            //else
            //{
                return rows;
            //}
        }
                
        public static string CleanHTML(string html)
        {
            // Tags
            html = Regex.Replace(html, @"<[^>]+>", "").Trim().Replace(System.Environment.NewLine, "").Replace("&#039;", "'").Replace("&quot;", "'").Replace("\n", "");
            // Double spaces
            html = Regex.Replace(html, @"[ ]{2,}", @" ");

            // Line
            html = html.Replace("â€”", " - ");
            // Pound
            html = html.Replace("Â£", "£");
            // Euro
            html = html.Replace("â‚¬", "€");
            // Open/closing quotes
            html = html.Replace("â€œ", "'");
            html = html.Replace("â€™", "'");
            html = html.Replace("â€", "'");
            html = html.Replace("\\\"", "'");
            html = html.Replace("&#x27;", "'");
            // Ampersand
            html = html.Replace("&amp;", "&");
        
            return html.Trim();
        }

        public static string SepGraph(string input)
        {
            string output = "";

            foreach (char b in input)
            {
                string transform = b.ToString();

                output += b switch
                {
                    ' ' => "&nbsp;",
                    '!' => "&#xE261;",
                    '\"' => "&#xE262;",
                    '£' => "&#xE263;",
                    '$' => "&#xE264;",
                    '%' => "&#xE265;",
                    '&' => "&#xE266;",
                    '\'' => "&#xE267;",
                    '(' => "&#xE268;",
                    ')' => "&#xE269;",
                    '*' => "&#xE26A;",
                    '+' => "&#xE26B;",
                    ',' => "&#xE26C;",
                    '-' => "&#xE26D;",
                    '.' => "&#xE26E;",
                    '/' => "&#xE26F;",
                    '0' => "&#xE270;",
                    '1' => "&#xE271;",
                    '2' => "&#xE272;",
                    '3' => "&#xE273;",
                    '4' => "&#xE274;",
                    '5' => "&#xE275;",
                    '6' => "&#xE276;",
                    '7' => "&#xE277;",
                    '8' => "&#xE278;",
                    '9' => "&#xE279;",
                    ':' => "&#xE27A;",
                    ';' => "&#xE27B;",
                    '<' => "&#xE27C;",
                    '=' => "&#xE27D;",
                    '>' => "&#xE27E;",
                    '?' => "&#xE27F;",
                    '_' => "&#xE280;",
                    'a' => "&#xE281;",
                    'b' => "&#xE282;",
                    'c' => "&#xE283;",
                    'd' => "&#xE284;",
                    'e' => "&#xE285;",
                    'f' => "&#xE286;",
                    'g' => "&#xE287;",
                    'h' => "&#xE288;",
                    'i' => "&#xE289;",
                    'j' => "&#xE28A;",
                    'k' => "&#xE28B;",
                    'l' => "&#xE28C;",
                    'm' => "&#xE28D;",
                    'n' => "&#xE28E;",
                    'o' => "&#xE28F;",
                    'p' => "&#xE290;",
                    'q' => "&#xE291;",
                    'r' => "&#xE292;",
                    's' => "&#xE293;",
                    't' => "&#xE294;",
                    'u' => "&#xE295;",
                    'v' => "&#xE296;",
                    'w' => "&#xE297;",
                    'x' => "&#xE298;",
                    'y' => "&#xE299;",
                    'z' => "&#xE29A;",
                    '{' => "&#xE29B;",
                    '|' => "&#xE29C;",
                    '}' => "&#xE29D;",
                    '~' => "&#xE29E;",
                    '@' => "&#xE29F;",
                    _ => b.ToString(),
                };
            }

            return output;
        }

       

        public static string BlockGraph(string input)
        {
            string output = "";

            foreach (char b in input)
            {
                string transform = b.ToString();

                output += b switch
                {
                    ' ' => "&nbsp;",
                    '!' => "&#xE201;",
                    '\"' => "&#xE202;",
                    '£' => "&#xE203;",
                    '$' => "&#xE204;",
                    '%' => "&#xE205;",
                    '&' => "&#xE206;",
                    '\'' => "&#xE207;",
                    '(' => "&#xE208;",
                    ')' => "&#xE209;",
                    '*' => "&#xE20A;",
                    '+' => "&#xE20B;",
                    ',' => "&#xE20C;",
                    '-' => "&#xE20D;",
                    '.' => "&#xE20E;",
                    '/' => "&#xE20F;",
                    '0' => "&#xE210;",
                    '1' => "&#xE211;",
                    '2' => "&#xE212;",
                    '3' => "&#xE213;",
                    '4' => "&#xE214;",
                    '5' => "&#xE215;",
                    '6' => "&#xE216;",
                    '7' => "&#xE217;",
                    '8' => "&#xE218;",
                    '9' => "&#xE219;",
                    ':' => "&#xE21A;",
                    ';' => "&#xE21B;",
                    '<' => "&#xE21C;",
                    '=' => "&#xE21D;",
                    '>' => "&#xE21E;",
                    '?' => "&#xE21F;",
                    '_' => "&#xE220;",
                    'a' => "&#xE221;",
                    'b' => "&#xE222;",
                    'c' => "&#xE223;",
                    'd' => "&#xE224;",
                    'e' => "&#xE225;",
                    'f' => "&#xE226;",
                    'g' => "&#xE227;",
                    'h' => "&#xE228;",
                    'i' => "&#xE229;",
                    'j' => "&#xE22A;",
                    'k' => "&#xE22B;",
                    'l' => "&#xE22C;",
                    'm' => "&#xE22D;",
                    'n' => "&#xE22E;",
                    'o' => "&#xE22F;",
                    'p' => "&#xE230;",
                    'q' => "&#xE231;",
                    'r' => "&#xE232;",
                    's' => "&#xE233;",
                    't' => "&#xE234;",
                    'u' => "&#xE235;",
                    'v' => "&#xE236;",
                    'w' => "&#xE237;",
                    'x' => "&#xE238;",
                    'y' => "&#xE239;",
                    'z' => "&#xE23A;",
                    '{' => "&#xE23B;",
                    '|' => "&#xE23C;",
                    '}' => "&#xE23D;",
                    '~' => "&#xE23E;",
                    '@' => "&#xE23F;",
                    _ => b.ToString(),
                };
            }

            return output;
        }
    }
}