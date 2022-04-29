using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.ServiceModel.Syndication;
using System.IO;
using System.Xml;
using HtmlAgilityPack;

namespace PagesFromCeefax
{
    public static class Utility
    {
        // Mixture of string parsing utilities and graphics character handling
        public static List<string> ParseParagraph(string content)
        {
            return ParseParagraph(content, 39, 39);
        }

        public static List<string> ParseParagraph(string content, int lineLength, int firstLineOverride)
        {
            List<string> rows = new List<String>();
            content += "</p>";  // In case there isn't already an ending </p>
            content = Utility.CleanHTML(content.Substring(0, content.IndexOf("</p>")));

            String[] words = content.Split(' ');
            string currentLine = "";
            bool invalidText = false;
            int effectiveLineLength = firstLineOverride;
            
            foreach (string currentWord in words)
            {
                if (currentWord.ToUpper() == "JAVASCRIPT" || currentWord.ToUpper() == "(CSS)")
                {
                    invalidText = true;
                }

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

            if (invalidText || currentLine.EndsWith(":") || currentLine.EndsWith("&hellip;"))
            {
                return new List<string>();
            }
            else
            {
                return rows;
            }
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

            return html;
        }

        public static string SepGraph(string input)
        {
            string output = "";

            foreach (char b in input)
            {
                string transform = b.ToString();

                switch (b)
                {
                    case ' ':
                        output += "&nbsp;";
                        break;
                    case '!':
                        output += "&#xE261;";
                        break;
                    case '\"':
                        output += "&#xE262;";
                        break;
                    case '£':
                        output += "&#xE263;";
                        break;
                    case '$':
                        output += "&#xE264;";
                        break;
                    case '%':
                        output += "&#xE265;";
                        break;
                    case '&':
                        output += "&#xE266;";
                        break;
                    case '\'':
                        output += "&#xE267;";
                        break;
                    case '(':
                        output += "&#xE268;";
                        break;
                    case ')':
                        output += "&#xE269;";
                        break;
                    case '*':
                        output += "&#xE26A;";
                        break;
                    case '+':
                        output += "&#xE26B;";
                        break;
                    case ',':
                        output += "&#xE26C;";
                        break;
                    case '-':
                        output += "&#xE26D;";
                        break;
                    case '.':
                        output += "&#xE26E;";
                        break;
                    case '/':
                        output += "&#xE26F;";
                        break;
                    case '0':
                        output += "&#xE270;";
                        break;
                    case '1':
                        output += "&#xE271;";
                        break;
                    case '2':
                        output += "&#xE272;";
                        break;
                    case '3':
                        output += "&#xE273;";
                        break;
                    case '4':
                        output += "&#xE274;";
                        break;
                    case '5':
                        output += "&#xE275;";
                        break;
                    case '6':
                        output += "&#xE276;";
                        break;

                    case '7':
                        output += "&#xE277;";
                        break;
                    case '8':
                        output += "&#xE278;";
                        break;
                    case '9':
                        output += "&#xE279;";
                        break;
                    case ':':
                        output += "&#xE27A;";
                        break;
                    case ';':
                        output += "&#xE27B;";
                        break;
                    case '<':
                        output += "&#xE27C;";
                        break;
                    case '=':
                        output += "&#xE27D;";
                        break;
                    case '>':
                        output += "&#xE27E;";
                        break;
                    case '?':
                        output += "&#xE27F;";
                        break;
                    case '_':
                        output += "&#xE280;";
                        break;
                    case 'a':
                        output += "&#xE281;";
                        break;
                    case 'b':
                        output += "&#xE282;";
                        break;
                    case 'c':
                        output += "&#xE283;";
                        break;
                    case 'd':
                        output += "&#xE284;";
                        break;
                    case 'e':
                        output += "&#xE285;";
                        break;
                    case 'f':
                        output += "&#xE286;";
                        break;
                    case 'g':
                        output += "&#xE287;";
                        break;
                    case 'h':
                        output += "&#xE288;";
                        break;
                    case 'i':
                        output += "&#xE289;";
                        break;
                    case 'j':
                        output += "&#xE28A;";
                        break;
                    case 'k':
                        output += "&#xE28B;";
                        break;
                    case 'l':
                        output += "&#xE28C;";
                        break;

                    case 'm':
                        output += "&#xE28D;";
                        break;
                    case 'n':
                        output += "&#xE28E;";
                        break;
                    case 'o':
                        output += "&#xE28F;";
                        break;
                    case 'p':
                        output += "&#xE290;";
                        break;
                    case 'q':
                        output += "&#xE291;";
                        break;
                    case 'r':
                        output += "&#xE292;";
                        break;
                    case 's':
                        output += "&#xE293;";
                        break;
                    case 't':
                        output += "&#xE294;";
                        break;
                    case 'u':
                        output += "&#xE295;";
                        break;
                    case 'v':
                        output += "&#xE296;";
                        break;
                    case 'w':
                        output += "&#xE297;";
                        break;
                    case 'x':
                        output += "&#xE298;";
                        break;
                    case 'y':
                        output += "&#xE299;";
                        break;
                    case 'z':
                        output += "&#xE29A;";
                        break;
                    case '{':
                        output += "&#xE29B;";
                        break;
                    case '|':
                        output += "&#xE29C;";
                        break;
                    case '}':
                        output += "&#xE29D;";
                        break;
                    case '~':
                        output += "&#xE29E;";
                        break;
                    case '@':
                        output += "&#xE29F;";
                        break;

                    default:
                        output += b.ToString();
                        break;
                }
            }

            return output;
        }

       

        public static string BlockGraph(string input)
        {
            string output = "";

            foreach (char b in input)
            {
                string transform = b.ToString();

                switch (b)
                {
                    case ' ':
                        output += "&nbsp;";
                        break;
                    case '!':
                        output += "&#xE201;";
                        break;
                    case '\"':
                        output += "&#xE202;";
                        break;
                    case '£':
                        output += "&#xE203;";
                        break;
                    case '$':
                        output += "&#xE204;";
                        break;
                    case '%':
                        output += "&#xE205;";
                        break;
                    case '&':
                        output += "&#xE206;";
                        break;
                    case '\'':
                        output += "&#xE207;";
                        break;
                    case '(':
                        output += "&#xE208;";
                        break;
                    case ')':
                        output += "&#xE209;";
                        break;
                    case '*':
                        output += "&#xE20A;";
                        break;
                    case '+':
                        output += "&#xE20B;";
                        break;
                    case ',':
                        output += "&#xE20C;";
                        break;
                    case '-':
                        output += "&#xE20D;";
                        break;
                    case '.':
                        output += "&#xE20E;";
                        break;
                    case '/':
                        output += "&#xE20F;";
                        break;
                    case '0':
                        output += "&#xE210;";
                        break;
                    case '1':
                        output += "&#xE211;";
                        break;
                    case '2':
                        output += "&#xE212;";
                        break;
                    case '3':
                        output += "&#xE213;";
                        break;
                    case '4':
                        output += "&#xE214;";
                        break;
                    case '5':
                        output += "&#xE215;";
                        break;
                    case '6':
                        output += "&#xE216;";
                        break;

                    case '7':
                        output += "&#xE217;";
                        break;
                    case '8':
                        output += "&#xE218;";
                        break;
                    case '9':
                        output += "&#xE219;";
                        break;
                    case ':':
                        output += "&#xE21A;";
                        break;
                    case ';':
                        output += "&#xE21B;";
                        break;
                    case '<':
                        output += "&#xE21C;";
                        break;
                    case '=':
                        output += "&#xE21D;";
                        break;
                    case '>':
                        output += "&#xE21E;";
                        break;
                    case '?':
                        output += "&#xE21F;";
                        break;
                    case '_':
                        output += "&#xE220;";
                        break;
                    case 'a':
                        output += "&#xE221;";
                        break;
                    case 'b':
                        output += "&#xE222;";
                        break;
                    case 'c':
                        output += "&#xE223;";
                        break;
                    case 'd':
                        output += "&#xE224;";
                        break;
                    case 'e':
                        output += "&#xE225;";
                        break;
                    case 'f':
                        output += "&#xE226;";
                        break;
                    case 'g':
                        output += "&#xE227;";
                        break;
                    case 'h':
                        output += "&#xE228;";
                        break;
                    case 'i':
                        output += "&#xE229;";
                        break;
                    case 'j':
                        output += "&#xE22A;";
                        break;
                    case 'k':
                        output += "&#xE22B;";
                        break;
                    case 'l':
                        output += "&#xE22C;";
                        break;

                    case 'm':
                        output += "&#xE22D;";
                        break;
                    case 'n':
                        output += "&#xE22E;";
                        break;
                    case 'o':
                        output += "&#xE22F;";
                        break;
                    case 'p':
                        output += "&#xE230;";
                        break;
                    case 'q':
                        output += "&#xE231;";
                        break;
                    case 'r':
                        output += "&#xE232;";
                        break;
                    case 's':
                        output += "&#xE233;";
                        break;
                    case 't':
                        output += "&#xE234;";
                        break;
                    case 'u':
                        output += "&#xE235;";
                        break;
                    case 'v':
                        output += "&#xE236;";
                        break;
                    case 'w':
                        output += "&#xE237;";
                        break;
                    case 'x':
                        output += "&#xE238;";
                        break;
                    case 'y':
                        output += "&#xE239;";
                        break;
                    case 'z':
                        output += "&#xE23A;";
                        break;
                    case '{':
                        output += "&#xE23B;";
                        break;
                    case '|':
                        output += "&#xE23C;";
                        break;
                    case '}':
                        output += "&#xE23D;";
                        break;
                    case '~':
                        output += "&#xE23E;";
                        break;
                    case '@':
                        output += "&#xE23F;";
                        break;

                    default:
                        output += b.ToString();
                        break;
                }
            }

            return output;
        }
    }
}