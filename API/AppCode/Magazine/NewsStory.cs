using System;
using System.Text;
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
                ?? doc.DocumentNode.SelectNodes("//article/div/p//span")                                // sport page
                ?? doc.DocumentNode.SelectNodes("//article//div/p");                                    // video story

            int pageNo = 0;
            if (mainlines != null)
            {
                // Break into sentences (one paragraph = one sentence)
                StringBuilder allText = new StringBuilder();
                foreach (var l in mainlines)
                {
                    allText.Append(l.InnerText + " ");
                }

                foreach (var l in allText.ToString().Split(". "))
                {
                    if (l.Trim() != String.Empty)
                    {
                        List<string> newChunk = Utility.ParseParagraph(l + ". ");

                        if (newChunk.Count > 0)
                        {
                            if (Headline.Count + Body[pageNo].Count + newChunk.Count - 1 > maxLines)
                            {
                                // The current paragraph will overflow the page
                                pageNo++;
                                if (pageNo == 2)
                                {
                                    // We only want two pages, so stop now
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
}

