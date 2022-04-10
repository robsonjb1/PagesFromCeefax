using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PagesFromCeefax
{
    public class TeletextPageWeather : TeletextPage
    {
        public TeletextPageWeather(MagazineContent mc) : base(mc)
        {
        }

        public StringBuilder CreateWeather(int sectionPage, string sectionTitle, string sectionText)
        {
            StringBuilder sb = new StringBuilder();
            sectionTitle = sectionTitle.ToUpper() + string.Join("", Enumerable.Repeat("&nbsp;", 36 - sectionTitle.Length));

            sb.Append(Graphics.HeaderWeather);
            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">{sectionTitle}</span><span class=\"ink{(int)Mode7Colour.White}\">{sectionPage}/3</p>");
            
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

            return BuildTeletextPage(sb);
        }

    }
}