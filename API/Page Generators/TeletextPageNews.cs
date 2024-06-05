using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public interface ITeletextPageNews
{
    public List<StringBuilder> CreateNewsPage(CeefaxSectionType sectionName);
    public StringBuilder CreateNewsInBrief(CeefaxSectionType sectionName);
}

public class TeletextPageNews : ITeletextPageNews
{
    private readonly ICeefaxContent _mc;

    public TeletextPageNews(ICeefaxContent mc)
    {
        _mc = mc;
    }

    #region Public Methods
    public List<StringBuilder> CreateNewsPage(CeefaxSectionType sectionName)
    {
        CeefaxSection section = _mc.Sections.Find(z => z.Name == sectionName)!;
        List<StringBuilder> content = new();

        // Loop through each story and generate a news page
        int storyCount = 1;
        foreach (NewsStory story in _mc.StoryList.FindAll(z => z.SectionName == sectionName && z.Body.Count > 0))
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
        CeefaxSection section = _mc.Sections.Find(z => z.Name == sectionName)!;
        StringBuilder sb = new();

        TextReader tr = new StringReader(_mc.UrlCache.Find(l => l.Location == _mc.Sections.Find(z => z.Name == sectionName)!.Feed)!.Content!);
        SyndicationFeed feed = SyndicationFeed.Load(XmlReader.Create(tr));

        sb.Append(section.Header);
        sb.AppendLine($"<p><span class=\"indent ink{(int)section.HeadingCol!}\">OTHER NEWS IN BRIEF...</span></p>");

        int rows = 0;
        foreach (SyndicationItem item in feed.Items)
        {
            if (!_mc.StoryList.Exists(z => z.Link == item.Links[0].Uri)
                && !item.Title.Text.Contains("VIDEO:", StringComparison.CurrentCulture)
                && item.Summary is not null)
            {
                List<string> title = Utility.ParseParagraph(item.Title.Text.Trim() + ".");
                List<string> summary = Utility.ParseParagraph(item.Summary.Text);
                if (rows + title.Count + summary.Count < 18)
                {
                    if (rows > 0)
                    {
                        rows++;
                        sb.Append("<br>");
                    }

                    // Title
                    foreach (string line in title)
                    {
                        sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.White} indent\">{line}</span></p>");
                    }

                    // Summary
                    foreach (string line in summary)
                    {
                        sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Cyan} indent\">{line}</span></p>");
                    }
                    rows += summary.Count + title.Count;
                    
                    // Add this to the global list of stories, it will never have summary content because the feed URL is
                    // never visited - but it will stop other news in brief sections picking up the same article
                    _mc.StoryList.Add(new NewsStory(sectionName, "NEWS IN BRIEF - DO NOT DISPLAY", item.Links[0].Uri));
                }
            }
        }

        // Pad lines to the end
        Utility.PadLines(sb, 18 - rows);
        
        // Display bespoke footer
        Utility.FooterText(sb, section, true);

        return sb;
    }
    #endregion

    #region Private Methods
    private static StringBuilder CreateNewsPage(CeefaxSection section, NewsStory story, bool isLastStory)
    {
        StringBuilder sb = new();
        sb.Append(section.Header);

        bool firstParagraph = true;
        Mode7Colour bodyCol = Mode7Colour.White;

        // Headline
        foreach (string line in story.Headline)
        {
            sb.AppendLine($"<p><span class=\"ink{(int)section.HeadingCol!} indent\">{line}</span></p>");
        }

        // Story
        foreach (string line in story.Body)
        {
            if (line == String.Empty)
            {
                if (!firstParagraph)
                {
                    sb.AppendLine("<br>");
                    bodyCol = Mode7Colour.Cyan;
                }
                firstParagraph = false;
            }
            else
            {
                sb.AppendLine($"<p><span class=\"ink{(int)bodyCol} indent\">{line}</span></p>");
            }
        }

        // Pad lines to the end
        Utility.PadLines(sb, 20 - story.Headline.Count - story.Body.Count);

        // Display footer
        Utility.FooterText(sb, section, isLastStory && !section.HasNewsInBrief);

        return sb;
    }
    #endregion
}