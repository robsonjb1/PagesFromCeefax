using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public class TeletextPageNews(CeefaxContent cc, WeatherData wd, MarketsData md)
{
    private readonly CeefaxContent _cc = cc;
    private readonly WeatherData _wd = wd;
    private readonly MarketsData _md = md;
    
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
       
        if(_md != null && _wd != null)
        {
            // Create the weather and markets section 
            List<string> weatherMarkets = [];
            weatherMarkets.Add(string.Empty);
            
            foreach(string line in Utility.ParseParagraph("WEATHER " + _wd.Forecasts[0].Body[.._wd.Forecasts[0].Body.IndexOf('.')]))
            {
                if(line.StartsWith("WEATHER "))
                {
                    weatherMarkets.Add($"[{TeletextControl.AlphaYellow}]WEATHER[{TeletextControl.AlphaWhite}]" + line[8..]);
                }
                else
                {
                    weatherMarkets.Add(" " + line);
                }
            }
            
            weatherMarkets.Add(string.Empty);

            MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name == "UKX");
            TeletextControl rateColour = mr.Movement.StartsWith('-') ? TeletextControl.AlphaRed : TeletextControl.AlphaGreen;
            string indexes = $"FTSE-100[{rateColour}]{Utility.PadRightWithTrunc(mr.Value[..mr.Value.IndexOf('.')],6)}";
            
            MarketRecord br = _md.Markets.FirstOrDefault(z => z.Name == "UK-10YRBOND");
            TeletextControl bondColour = br.Movement.StartsWith('-') ? TeletextControl.AlphaGreen : TeletextControl.AlphaRed;
            string bonds = $"[{bondColour}]{Utility.PadRightWithTrunc(br.Value, 7)}";
            
            weatherMarkets.Add($"[{TeletextControl.AlphaYellow}]MARKETS[{TeletextControl.AlphaWhite}]{indexes}[{TeletextControl.AlphaWhite}]10YR-BND{bonds}");

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

                    if (rows + HeadlineRows.Count + SummaryRows.Count < 19 - weatherMarkets.Count)
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

            // Add weather markets
            foreach(string line in weatherMarkets)
            {
                sb.AppendLine(line);
            }

            // Pad lines to the end
            sb.PadLines(19 - weatherMarkets.Count - rows);

            sb.Append($"[{TeletextControl.AlphaBlue}][{TeletextControl.NewBackground}][{TeletextControl.AlphaYellow}]");
            sb.AppendLine("Home news coming up next >>>");
        }

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