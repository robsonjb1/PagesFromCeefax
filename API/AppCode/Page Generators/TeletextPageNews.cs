using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;

namespace PagesFromCeefax
{
    public class TeletextPageNews : TeletextPage
    {
        public TeletextPageNews(MagazineContent mc) : base(mc)
        {
        }

        public StringBuilder OutputNewsSection(ref int PageNo, StringBuilder header, Mode7Colour headingCol, MagazineSectionType sectionName, string promoFooter, Mode7Colour promoPaper, Mode7Colour promoInk)
        {
            StringBuilder content = new StringBuilder();

            // Loop through each story and generate a news page
            int storyCount = 1;
            foreach (NewsStory story in _mc.StoryList.FindAll(z => z.SectionName == sectionName && z.Body.Count > 0))
            {
                content.Append(OutputNewsPage(ref PageNo, header, headingCol, story, (storyCount == (_mc.Sections.Find(z => z.Name == sectionName).TotalStories) ? promoFooter : ""), promoPaper, promoInk));
                storyCount++;
            }

            return content;
        }

        private StringBuilder OutputNewsPage(
            ref int pageNo, StringBuilder header, Mode7Colour headingCol, NewsStory story, string promoFooter, Mode7Colour promoPaper, Mode7Colour promoInk)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header.ToString());

            bool firstParagraph = true;
            Mode7Colour bodyCol = Mode7Colour.White;
            foreach (string line in story.Summary)
            {
                sb.AppendLine(String.Format("<p><span class=\"ink{0} indent\">{1}</span></p>", Convert.ToInt32(headingCol), line));
            }
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
                    sb.AppendLine(String.Format("<p><span class=\"ink{0} indent\">{1}</span></p>", Convert.ToInt32(bodyCol), line));
                }
            }

            for (int i = 0; i < 20 - (story.Summary.Count + story.Body.Count); i++)
            {
                sb.Append("<br>");
            }

            if (promoFooter == "")
            {
                sb.Append(String.Format("<p><span class=\"paper{0} ink{1}\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>", Convert.ToInt32(promoPaper), Convert.ToInt32(promoInk)));
            }
            else
            {
                sb.Append(String.Format("<p><span class=\"paper{0} ink{1}\">&nbsp;&nbsp;{2}</span></p>", Convert.ToInt32(promoPaper), Convert.ToInt32(promoInk), promoFooter + string.Join("", Enumerable.Repeat("&nbsp;", 37 - promoFooter.Length))));
            }

            return BuildTeletextPage(ref pageNo, sb);
        }

        public StringBuilder OutputNewsInBrief(
            ref int pageNo, StringBuilder header, Mode7Colour headingCol, MagazineSectionType sectionName, string promoFooter, Mode7Colour promoPaper, Mode7Colour promoInk)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(header);
            sb.AppendLine("<p><span class=\"indent ink" + Convert.ToInt32(headingCol) + "\">OTHER NEWS IN BRIEF...</span></p>");

            SyndicationFeed feed = Utility.ReadRSSFeed(_mc.UrlCache.Find(l => l.Location == _mc.Sections.Find(z => z.Name == sectionName).Feed).Content);

            int rows = 0;
          
            foreach (SyndicationItem item in feed.Items)
            {
                if (!_mc.StoryList.Exists(z => z.Link == item.Links[0].Uri) && item.Title.Text.IndexOf("VIDEO:") == -1)
                {
                    List<string> title = Utility.ParseParagraph(item.Title.Text);
                    List<string> summary = Utility.ParseParagraph(item.Summary.Text);
                    if (rows + title.Count + summary.Count < 17)
                    {
                        if (rows > 0)
                        {
                            rows++;
                            sb.Append("<br>");
                        }

                        foreach (string line in title)
                        {
                            sb.AppendLine(String.Format("<p><span class=\"ink7 indent\">{0}</span></p>", line));
                        }

                        foreach (string line in summary)
                        {
                            sb.AppendLine(String.Format("<p><span class=\"ink5 indent\">{0}</span></p>", line));
                        }
                        rows += summary.Count + title.Count;
                        
                        // Add this to the global list of stories, it will never have summary content because the feed URL is
                        // never visited - but it will stop other news in brief sections picking up the same article
                        _mc.StoryList.Add(new NewsStory(sectionName, "NEWS IN BRIEF - DO NOT DISPLAY", item.Links[0].Uri));
                    }
                }
            }
            for (int i = 0; i < 18 - rows; i++)
            {
                sb.Append("<br>");
            }

            if (promoFooter == "")
            {
                sb.Append(String.Format("<p><span class=\"paper{0} ink{1}\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>", Convert.ToInt32(promoPaper), Convert.ToInt32(promoInk)));
            }
            else
            {
                sb.Append(String.Format("<p><span class=\"paper{0} ink{1}\">&nbsp;&nbsp;{2}</span></p>", Convert.ToInt32(promoPaper), Convert.ToInt32(promoInk), promoFooter + string.Join("", Enumerable.Repeat("&nbsp;", 37 - promoFooter.Length))));
            }

            return BuildTeletextPage(ref pageNo, sb);
        }
    }
}