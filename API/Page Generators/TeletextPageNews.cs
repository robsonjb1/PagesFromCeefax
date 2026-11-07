using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public class TeletextPageNews(CeefaxContent cc)
{
    private readonly CeefaxContent _cc = cc;

    #region Public Methods
    public List<StringBuilder> CreateNewsSection(CeefaxSectionType sectionName)
    {
        CeefaxSection section = _cc.Sections.Find(z => z.Name == sectionName)!;
        List<StringBuilder> content = [];

        // Loop through each story and generate a news page (excluding commentary articles)
        for(int pageCount = 1; pageCount <= section.TotalPages; pageCount++)
        {
            content.Add(CreateNewsInBrief(section, pageCount, section.TotalPages));
        }

        return content;
    }

    public StringBuilder CreateNewsHeadlines()
    {
        StringBuilder sb = new();
        sb.Append(Graphics.HeaderHeadlines);
        
        // List latest stories regardless of section. If a story is shown, do not mark it as included
        // It will be intentionally repeated on the relevant section page later
        int rows = 0;
        List<Uri> included = [];
        foreach (NewsStory story in _cc.StoryList.OrderByDescending(z => z.PublishDate))
        {
            if(!included.Exists(z => z == story.Link))
            {
                included.Add(story.Link);

                List<string> HeadlineRows = Utility.ParseParagraph(story.SectionName.ToString().ToUpper() + " " + story.Headline);
                List<string> SummaryRows = Utility.ParseParagraph(story.Summary);

                if (rows + HeadlineRows.Count + SummaryRows.Count < 19)
                {
                    if(rows > 0) // Separator row
                    {
                        rows++;
                        sb.AppendLine(string.Empty);
                    }
                    
                    CeefaxSection section = _cc.Sections.Find(z => z.Name == story.SectionName)!;
                    sb.AppendLine($"[{section.HeadingCol}]{story.SectionName.ToString().ToUpper()}[{TeletextControl.AlphaWhite}]" + HeadlineRows[0][(story.SectionName.ToString().Length+1)..]);
                    if(HeadlineRows.Count > 1)
                    {
                        for(int i=1; i<HeadlineRows.Count; i++)
                        {
                            sb.AppendLine($"[{TeletextControl.AlphaWhite}]{HeadlineRows[i]}");
                        }
                    }

                    SummaryRows.ForEach(l => sb.AppendLine($"[{TeletextControl.AlphaCyan}]{l}"));
                    rows += HeadlineRows.Count + SummaryRows.Count;
                }
            }
        }

        // Pad lines to the end
        sb.PadLines(19 - rows);

        sb.Append($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaYellow}]");
        sb.AppendLine("Home news coming up next >>>");

        return sb;
    }

    public StringBuilder CreateNewsInBrief(CeefaxSection section, int currentPage, int totalPages)
    {
        StringBuilder sb = new();

        sb.Append(section.Header);

        // Workaround for The Register
        if(section.Name == CeefaxSectionType.Register)
        {
            sb.AppendLine($"[{TeletextControl.AlphaWhite}]{Utility.PadLeftWithTrunc($"{currentPage}/{totalPages}", 39)}");
        }
        else
        {
            // Regular news page
            if(totalPages > 1)
            {
                sb.AppendLine($"[{section.HeadingCol}]{section.Name.ToString().ToUpper()} HEADLINES {" ".PadRight(22-section.Name.ToString().Length)} [{TeletextControl.AlphaWhite}] {currentPage}/{totalPages}");
            }
            else
            {
                sb.AppendLine($"[{section.HeadingCol}]{section.Name.ToString().ToUpper()} HEADLINES");
            }
        }
    
        int rows = 0;
        foreach (NewsStory story in _cc.StoryList.FindAll(z=> z.SectionName == section.Name).OrderByDescending(z => z.PublishDate))
        {
            // Check this same story hasn't already been included on a previous page
            if(_cc.StoryList.Exists(z => z.Link == story.Link && z.Included))
            {
                story.Included = true;
            }

            List<string> HeadlineRows = Utility.ParseParagraph(story.Headline);
            List<string> SummaryRows = Utility.ParseParagraph(story.Summary);

            if (!story.Included && (rows + HeadlineRows.Count + SummaryRows.Count < 18))
            {
                if (rows > 0) // Separator row
                {
                    rows++;
                    sb.AppendLine(string.Empty);
                }

                // Title
                HeadlineRows.ForEach(l => sb.AppendLine($"[{TeletextControl.AlphaWhite}]{l}"));
              
                // Summary
                SummaryRows.ForEach(l => sb.AppendLine($"[{TeletextControl.AlphaCyan}]{l}"));
                
                rows += SummaryRows.Count + HeadlineRows.Count;
              
                // Ensure no other future pages include this story
                story.Included = true;
            }
        }

        // Pad lines to the end
        sb.PadLines(18 - rows);
        
        // Display bespoke footer
        sb.FooterText(section, totalPages == 1 || currentPage == totalPages);

        return sb;
    }
    #endregion
}