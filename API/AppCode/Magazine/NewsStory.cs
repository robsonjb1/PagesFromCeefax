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
        public readonly List<string> Headline;

        public List<string>[] Body = new List<string>[3] { new List<string>(), new List<string>(), new List<String>() };

        public NewsStory(MagazineSectionType SectionName, string Headline, Uri Link)
        {
            this.SectionName = SectionName;
            this.Link = Link;
            this.Headline = Utility.ParseParagraph(Headline);
        }

        public void AddBody(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Parse story body - only to be called once URL has been retrieved
            var mainlines = doc.DocumentNode.SelectNodes("//article/*[@data-component='text-block']")   // news page
                ?? doc.DocumentNode.SelectNodes("//article/div/p/span")                                 // sport page
                ?? doc.DocumentNode.SelectNodes("//article//div/p");                                    // video story

            int pageNo = 0;

            if (mainlines != null)
            {
                foreach (var l in mainlines)
                {
                    List<string> newChunk = Utility.ParseParagraph(l.InnerText);

                    if (newChunk.Count > 0)
                    {
                        if (Headline.Count + Body[pageNo].Count + newChunk.Count - 1 > maxLines)
                        {
                            pageNo++;
                            if (pageNo == 2)
                            {
                                break;
                            }
                        }

                        Body[pageNo].Add("");
                        Body[pageNo].AddRange(newChunk);
                    }
                }
            }
        }
    }
}

