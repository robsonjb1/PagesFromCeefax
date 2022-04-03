using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace PagesFromCeefax
{
    public class NewsStory
    {
        const int maxLines = 18;

        public readonly MagazineSectionType SectionName;
        public readonly Uri Link;
        public readonly List<string> Summary;

        private List<string>? _body;
        
        public NewsStory(MagazineSectionType SectionName, string Summary, Uri Link)
        {
            this.SectionName = SectionName;
            this.Link = Link;
            this.Summary = Utility.ParseParagraph(Summary);
        }

        public List<string>? Body
        {
            get
            {
                return _body;
            }
        }

        public void AddBody(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Parse story body - only to be called once URL has been retrieved
            _body = new List<string>();

            var mainlines = doc.DocumentNode.SelectNodes("//article/*[@data-component='text-block']")
                ?? doc.DocumentNode.SelectNodes("//article/div/p/span");

            if (mainlines != null)
            {
                foreach (var l in mainlines)
                {
                    List<string> newChunk = Utility.ParseParagraph(l.InnerText);

                    if (newChunk.Count > 0)
                    {
                        if (this.Summary.Count + _body.Count + newChunk.Count - 1 > maxLines)
                        {
                            break;
                        }
                        else
                        {
                            _body.Add("");
                            _body.AddRange(newChunk);
                        }
                    }
                }
            }
        }
    }
}

