using System.Security.Principal;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public interface ITeletextPageNews
{
    public List<StringBuilder> CreateNewsSection(CeefaxSectionType sectionName);
    public StringBuilder CreateNewsInBrief(CeefaxSectionType sectionName);
}

public class TeletextPageNews : ITeletextPageNews
{
    private readonly ICeefaxContent _cc;

    public TeletextPageNews(ICeefaxContent cc)
    {
        _cc = cc;
    }

    #region Public Methods
    public List<StringBuilder> CreateNewsSection(CeefaxSectionType sectionName)
    {
        CeefaxSection section = _cc.Sections.Find(z => z.Name == sectionName)!;
        List<StringBuilder> content = new();

        // Loop through each story and generate a news page (excluding commentary articles)
        int storyCount = 1;
        foreach (NewsStory story in _cc.StoryList.FindAll(z => z.SectionName == sectionName && !z.Headline[^1].EndsWith('?') && z.Body.Count > 0))
        {
            content.Add(CreateNewsPage(section, story,
            isLastStory: storyCount == section.TotalStories));
            storyCount++;
        }

        if (section.HasNewsInBrief)
        {
            content.Add(CreateNewsInBrief(sectionName));
        }

        return content;
    }

    public StringBuilder CreateNewsInBrief(CeefaxSectionType sectionName)
    {
        CeefaxSection section = _cc.Sections.Find(z => z.Name == sectionName)!;
        StringBuilder sb = new();

        TextReader tr = new StringReader(_cc.UriCache.Find(l => l.Location == _cc.Sections.Find(z => z.Name == sectionName)!.Feed)!.ContentString!);
        SyndicationFeed feed = SyndicationFeed.Load(XmlReader.Create(tr));

        sb.Append(section.Header);
        sb.AppendLine($"[{section.HeadingCol}]OTHER NEWS IN BRIEF...");

        int rows = 0;
        foreach (SyndicationItem item in feed.Items)
        {
            if (!_cc.StoryList.Exists(z => z.Link == item.Links[0].Uri)
                && !item.Title.Text.Contains("VIDEO:", StringComparison.CurrentCulture)
                && item.Summary is not null)
            {
                List<string> title = Utility.ParseParagraph(item.Title.Text);
                List<string> summary = Utility.ParseParagraph(item.Summary.Text);
                if (rows + title.Count + summary.Count < 18)
                {
                    if (rows > 0)
                    {
                        rows++;
                        sb.AppendLine(String.Empty);
                    }

                    // Title
                    title.ForEach(l => sb.AppendLine($"[{TeletextControl.AlphaWhite}]{l}"));
                    
                    // Summary
                    summary.ForEach(l => sb.AppendLine($"[{TeletextControl.AlphaCyan}]{l}"));
                    
                    rows += summary.Count + title.Count;
                    
                    // Add this to the global list of stories, it will never have summary content because the feed URL is
                    // never visited - but it will stop other news in brief sections picking up the same article
                    _cc.StoryList.Add(new NewsStory(sectionName, "NEWS IN BRIEF - DO NOT DISPLAY", item.Links[0].Uri));
                }
            }
        }

        // Pad lines to the end
        sb.PadLines(18 - rows);
        
        // Display bespoke footer
        sb.FooterText(section, true);

        return sb;
    }
    #endregion

    #region Private Methods
    private static StringBuilder CreateNewsPage(CeefaxSection section, NewsStory story, bool isLastStory)
    {
        StringBuilder sb = new();
        sb.Append(section.Header);

        bool firstParagraph = true;
        TeletextControl bodyCol = TeletextControl.AlphaWhite;

        // Headline
        story.Headline.ForEach(l => sb.AppendLine($"[{section.HeadingCol}]{l}"));
        
        // Story
        foreach (string line in story.Body)
        {
            if (line == String.Empty)
            {
                if (!firstParagraph)
                {
                    sb.AppendLine(String.Empty);
                    bodyCol = TeletextControl.AlphaCyan;
                }
                firstParagraph = false;
            }
            else
            {
                sb.AppendLine($"[{bodyCol}]{line}");
            }
        }

        // Pad lines to the end
        sb.PadLines(20 - story.Headline.Count - story.Body.Count);

        // Display footer
        sb.FooterText(section, isLastStory && !section.HasNewsInBrief);

        return sb;
    }
    #endregion
}