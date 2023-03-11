using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators
{
    public interface ITeletextPageNews
    {
        public List<StringBuilder> CreateNewsSection(MagazineSectionType sectionName);
        public StringBuilder CreateNewsInBrief(MagazineSectionType sectionName);
    }

    public class TeletextPageNews : ITeletextPageNews
    {
        readonly IMagazineContent _mc;
    
        public TeletextPageNews(IMagazineContent mc)
        {
            _mc = mc;
        }


        #region Public Methods
        public List<StringBuilder> CreateNewsSection(MagazineSectionType sectionName)
        {
            MagazineSection section = _mc.Sections.Find(z => z.Name == sectionName)!;
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

        public StringBuilder CreateNewsInBrief(MagazineSectionType sectionName)
        {
            MagazineSection section = _mc.Sections.Find(z => z.Name == sectionName)!;
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
                    List<string> title = Utility.ParseParagraph(item.Title.Text + ".");
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
            for (int i = 0; i < 18 - rows; i++)
            {
                sb.Append("<br>");
            }

            // Display either the standard or a bespoke footer
            if (section.PromoFooter is null)
            {
                sb.Append($"<p><span class=\"paper{(int)section.PromoPaper!} ink{(int)section.PromoInk!}\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");
            }
            else
            {
                string promoFooter = section.PromoFooter + string.Join("", Enumerable.Repeat("&nbsp;", 37 - section.PromoFooter.Length));
                sb.Append($"<p><span class=\"paper{(int)section.PromoPaper!} ink{(int)section.PromoInk!}\">&nbsp;&nbsp;{promoFooter}</span></p>");
            }

            return sb;
        }
        #endregion

        #region Private Methods
        private static StringBuilder CreateNewsPage(MagazineSection section, NewsStory story, bool isLastStory)
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

            // Pad to the bottom of the page
            for (int i = 0; i < 20 - story.Headline.Count - story.Body.Count; i++)
            {
                sb.Append("<br>");
            }

            // Footer
            if (section.PromoFooter is not null
                && (isLastStory && !section.HasNewsInBrief))
            {
                string promoFooterPadded = section.PromoFooter + string.Join("", Enumerable.Repeat("&nbsp;", 37 - section.PromoFooter.Length));
                sb.Append($"<p><span class=\"paper{(int)section.PromoPaper!} ink{(int)section.PromoInk!}\">&nbsp;&nbsp;{promoFooterPadded!}</span></p>");
            }
            else
            {
                sb.Append($"<p><span class=\"paper{(int)section.PromoPaper!} ink{(int)section.PromoInk!}\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");
            }

            return sb;
        }
        #endregion
    }
}