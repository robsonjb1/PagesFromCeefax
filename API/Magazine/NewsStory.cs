using API.Architecture;
using HtmlAgilityPack;

namespace API.Magazine;

public class NewsStory
{
    const int maxLines = 18;

    public readonly CeefaxSectionType SectionName;
    public readonly Uri Link;
    public readonly List<string> Headline;
    public readonly List<string> MultiPageHeadline;
    public List<string> Body;

    public NewsStory(CeefaxSectionType SectionName, string Headline, Uri Link)
    {
        this.SectionName = SectionName;
        this.Link = Link;
        this.Headline = Utility.ParseParagraph(Headline);
        this.Body = new List<string>();
    }

    public void AddBody(string html)
    {
        HtmlDocument doc = new();
        doc.LoadHtml(html);

        // Parse story body - only to be called once URL has been retrieved
        var mainlines = doc.DocumentNode.SelectNodes("//article/*[@data-component='text-block']//p")   // news page
            ?? doc.DocumentNode.SelectNodes("//article//div/p");                                    // video story

        if (mainlines != null)
        {
            // Break into sentences (one paragraph = one sentence)
            List<string> storyText = new();
            foreach (var p in mainlines)
            {
                var tempLine = p.InnerText.Trim().Split(". ");
                storyText.AddRange(tempLine);
            }
            
            foreach (var l in storyText)
            {
			    if (l != String.Empty)
                {
                    List<string> newChunk = Utility.ParseParagraph(l);

                    if (newChunk.Count > 0)
                    {
                        if (Headline.Count + Body.Count + newChunk.Count - 1 > maxLines)
                        {
                            // The current paragraph will overflow the page
                           break;
                        }

                        Body.Add("");
                        Body.AddRange(newChunk);
                    }
                }
            }
        }
    }
}

