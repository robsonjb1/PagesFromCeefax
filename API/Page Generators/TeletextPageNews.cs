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

    public StringBuilder CreateNewsInBrief(CeefaxSection section, int currentPage, int totalPages)
    {
        StringBuilder sb = new();

        sb.Append(section.Header);
        if(totalPages > 1)
        {
            sb.AppendLine($"[{section.HeadingCol}]{section.Name.ToString().ToUpper()} HEADLINES... {" ".PadRight(19-section.Name.ToString().Length)} [{TeletextControl.AlphaWhite}] {currentPage}/{totalPages}");
        }
        else
        {
            sb.AppendLine($"[{section.HeadingCol}]{section.Name.ToString().ToUpper()} HEADLINES...");
        }
        
        int rows = 0;
        foreach (NewsStory story in _cc.StoryList.FindAll(z=> z.SectionName == section.Name).OrderByDescending(z => z.PublishDate))
        {
            // Check this same story hasn't already been included on a previous page
            if(_cc.StoryList.Exists(z => z.Link == story.Link && z.Included))
            {
                story.Included = true;
            }

            if (!story.Included && (rows + story.Headline.Count + story.Summary.Count < 18))
            {
                if (rows > 0)
                {
                    rows++;
                    sb.AppendLine(string.Empty);
                }

                // Title
                story.Headline.ForEach(l => sb.AppendLine($"[{TeletextControl.AlphaWhite}]{l}"));
                
                // Summary
                story.Summary.ForEach(l => sb.AppendLine($"[{TeletextControl.AlphaCyan}]{l}"));
                
                rows += story.Summary.Count + story.Headline.Count;
              
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