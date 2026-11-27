using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public class TeletextPageNews(CeefaxContent cc)
{
    private readonly CeefaxContent _cc = cc;
  
    #region Public Methods
    public List<StringBuilder> CreateNewsSection(CeefaxSectionType sectionName, int countOffset = 0, int totalPagesOffset = 0)
    {
        CeefaxSection section = _cc.Sections.Find(z => z.Name == sectionName)!;
        List<StringBuilder> content = [];

        // Loop through each story and generate a news page (excluding commentary articles)
        for(int pageCount = 1; pageCount <= section.TotalPages; pageCount++)
        {
            content.Add(CreateNewsInBrief(section, pageCount + countOffset, section.TotalPages + totalPagesOffset));
        }

        return content;
    }

    public StringBuilder CreateNewsInBrief(CeefaxSection section, int currentPage, int totalPages)
    {
        StringBuilder sb = new();

        // Workaround for The Register
        if(section.Name == CeefaxSectionType.Register)
        {
            sb.Append(section.Header.ToString().Replace("A/B", $"{currentPage}/{totalPages}"));
        }
        else
        {
            sb.Append(section.Header);
      
            // Regular news page
            if(totalPages > 1)
            {
                sb.AppendLine($"[{section.HeadingCol}]{section.Name.ToString().ToUpper()} HEADLINES{" ".PadRight(23-section.Name.ToString().Length)} [{TeletextControl.AlphaWhite}] {currentPage}/{totalPages}");
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

            if(HeadlineRows[0] != SummaryRows[0])
            {
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
        }

        // Pad lines to the end
        sb.PadLines(18 - rows);
        
        // Display bespoke footer
        sb.FooterText(section, totalPages == 1 || currentPage == totalPages);

        return sb;
    }
    #endregion
}