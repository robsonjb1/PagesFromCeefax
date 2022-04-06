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

        public StringBuilder OutputWeather(ref int pageNo, int sectionPage, string sectionTitle, string sectionText)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Graphics.HeaderWeather);

            sb.AppendLine(String.Format("<p><span class=\"ink6 indent\">{0}{1}</span><span class=\"ink7\">{2}/3</p>",
                sectionTitle.ToUpper(),
                string.Join("", Enumerable.Repeat("&nbsp;", 36 - sectionTitle.Length)),
                sectionPage));

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
                sb.AppendLine(String.Format("<p><span class=\"ink" + (firstLine ? "7" : "5") + " indent\">{0}</span></p>", line));
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
                sb.Append("<p><span class=\"ink4\">Data: BBC Weather Centre/Met Office</span></p>");
                sb.AppendLine("<br>");
            }

            sb.Append("<p><span class=\"paper1 ink6\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");

            return BuildTeletextPage(ref pageNo, sb);
        }

    }
}