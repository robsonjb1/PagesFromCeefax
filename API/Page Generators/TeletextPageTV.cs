using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public interface ITeletextPageTV
{
    public List<StringBuilder> CreateSchedule(CeefaxSectionType sectionName);
}

public class TeletextPageTV : ITeletextPageTV
{
    private readonly ICeefaxContent _cc;
    private readonly ITVListingData _ld;

    public TeletextPageTV(ICeefaxContent cc, ITVListingData listings)
    {
        _cc = cc;
        _ld = listings;
    }

    #region Public Methods
    public List<StringBuilder> CreateSchedule(CeefaxSectionType channel)
    {
        StringBuilder page1 = new();
        StringBuilder page2 = new();
       
        if(_ld.IsValid)             // Only construct the page if we have valid data
        {
            // Create a two page schedule for each channel
            int endTime = CreateSinglePage(page1, channel, 1800, false); // Find first show on or immediately after the start time
            CreateSinglePage(page2, channel, endTime, true); // Feed the end time from page 1 into the start time of page 2
        }

        return new List<StringBuilder> {page1, page2};
    }

    private string FormatDisplayTime(string time)
    {
        if(time == String.Empty)
        {
            return "0000";
        } else {
            return $"{time.Split(":")[0].PadLeft(2,'0')}{time.Split(":")[1].PadLeft(2,'0')}";
        }
    }

    private bool OnlyShowHeadline(string title)
    {
        return title.Contains("BBC NEWS") || title.Contains("BBC WEEKEND NEWS") ||
            title.Contains("SOUTH EAST TODAY") || title.Contains("WEATHER");
    }

    public int CreateSinglePage(StringBuilder pageSb, CeefaxSectionType channel, int startTime, bool exactMatch)
    {
        StringBuilder sb = new();
        CeefaxSection section = _cc.Sections.Find(z => z.Name == channel)!;

        // Create the appropriate channel header logo
        string ident = Graphics.HeaderTV.ToString();
        switch(channel)
        {
            case CeefaxSectionType.TVScheduleBBC1:
                ident = ident.Replace("{ChannelTop}", Utility.BlockGraph("(@ ")).Replace("{ChannelBottom}", Utility.BlockGraph("_@0"));
                break;
            case CeefaxSectionType.TVScheduleBBC2:
                ident = ident.Replace("{ChannelTop}", Utility.BlockGraph("bs@")).Replace("{ChannelBottom}", Utility.BlockGraph("jup"));
                break;
            case CeefaxSectionType.TVScheduleBBC4:
                ident = ident.Replace("{ChannelTop}", Utility.BlockGraph("@h4")).Replace("{ChannelBottom}", Utility.BlockGraph("£k7"));
                break;
            default:
                break;
        }

        ChannelSchedule schedule = _ld.Schedules.Find(z => z.Channel == channel);
        
        sb.Append(ident.Replace("{DayOfWeek}", schedule.Day.PadHtmlRight(15)));
        
        bool startListing = false;
        int lineCount = 0;
        string actualStartTime = "";        // The time of the first show found at or after the supplied time
        string timeOfLastPageEntry = "";

        foreach (var show in schedule.Listings)
        {
            int comboTime = Convert.ToInt32(FormatDisplayTime(show.StartTime));
            
            if(((!exactMatch && comboTime >= startTime) || (exactMatch && comboTime == startTime))
                && actualStartTime == String.Empty)
            {
                startListing = true;
                actualStartTime = show.StartTime;
            }            

            if(startListing)
            {
                var titleLines = Utility.ParseParagraph(show.Title, 34, 34, false);
                var bodyLines = Utility.ParseParagraph(show.Description, 34, 34, false);
                timeOfLastPageEntry = show.StartTime;

                // Don't show a description for news or weather
                if(OnlyShowHeadline(show.Title))
                {
                    bodyLines = new List<string>();
                }

                if(lineCount + titleLines.Count + bodyLines.Count > 18)
                {
                    // We will run off the end of the page
                    break;
                }

                lineCount = lineCount + titleLines.Count + bodyLines.Count;

                sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">");
                sb.AppendLine($"{FormatDisplayTime(show.StartTime)} </span><span class=\"ink{(int)Mode7Colour.White}\">{titleLines[0]}</span>");
                sb.AppendLine("</span></p>");
                
                // Output show title
                if(titleLines.Count > 1)
                {
                    for(int i = 1; i < titleLines.Count; i++) 
                    {
                        sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.White} indent\">");
                        sb.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{titleLines[i]}</span></p>");
                    }
                }
                
                // Show description
                for(int i = 0; i < bodyLines.Count; i++) 
                {
                    sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Cyan} indent\">");
                    sb.AppendLine($"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{bodyLines[i]}</span></p>");
                }
            }
        }

        // Ensure we pad any remaining lines until we get to the footer position
        Utility.PadLines(sb, 18-lineCount);
        
        // Display either the standard or bespoke footer (only valid for last page, where we specifically match on the previous page end time)
        Utility.FooterText(sb, section, exactMatch);
        
        // Only now do we now the true timespan. The end time shown is from the show that couldn't fit on this page.
        pageSb.Append(sb.Replace("{TimeSpan}", $"{FormatDisplayTime(actualStartTime)}-{FormatDisplayTime(timeOfLastPageEntry)}"));
        return Convert.ToInt32($"{FormatDisplayTime(timeOfLastPageEntry)}");
    }
    
    #endregion
}
