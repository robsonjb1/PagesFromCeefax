using System.Text;
using API.Architecture;
using API.Magazine;
using HtmlAgilityPack;

namespace API.PageGenerators;

public interface ITeletextPageSchedule
{
    public List<StringBuilder> CreateSchedule(MagazineSectionType sectionName);
}

public class TeletextPageSchedule : ITeletextPageSchedule
{
    private readonly IMagazineContent _mc;

    public TeletextPageSchedule(IMagazineContent mc)
    {
        _mc = mc;
    }

    #region Public Methods
    public List<StringBuilder> CreateSchedule(MagazineSectionType sectionName)
    {
        // Create a two page schedule for each channel
        StringBuilder page1 = new();
        int endTime = CreatePage(page1, sectionName, 1800, false); // Find first show on or immediately after the start time

        StringBuilder page2 = new();
        CreatePage(page2, sectionName, endTime, true); // Feed the end time from page 1 into the start time of page 2

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
        return title.Contains("BBC NEWS") || title.Contains("SOUTH EAST TODAY") || title.Contains("WEATHER");
    }

    public int CreatePage(StringBuilder pageSb, MagazineSectionType sectionName, int startTime, bool exactMatch)
    {
        StringBuilder sb = new();

        // Create the appropriate channel header logo
        string ident = Graphics.HeaderTV.ToString();
        switch(sectionName)
        {
            case MagazineSectionType.TVScheduleBBC1:
                ident = ident.Replace("{ChannelTop}", Utility.BlockGraph("(@ ")).Replace("{ChannelBottom}", Utility.BlockGraph("_@0"));
                break;
            case MagazineSectionType.TVScheduleBBC2:
                ident = ident.Replace("{ChannelTop}", Utility.BlockGraph("bs@")).Replace("{ChannelBottom}", Utility.BlockGraph("jup"));
                break;
            case MagazineSectionType.TVScheduleBBC4:
                ident = ident.Replace("{ChannelTop}", Utility.BlockGraph("@h4")).Replace("{ChannelBottom}", Utility.BlockGraph("£k7"));
                break;
            default:
                break;
        }

        string html = _mc.UrlCache.First(l => l.Location == _mc.Sections.First(z => z.Name == sectionName).Feed).Content;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        string day = doc.DocumentNode.SelectSingleNode("//time").InnerText.Trim();
        day = day.Substring(0, day.IndexOf(" ")).ToUpper();
        sb.Append(ident.Replace("{DayOfWeek}", string.Join("", Enumerable.Repeat("&nbsp;", 15 - day.Length)) + day));
                   
        var shows = doc.DocumentNode.SelectNodes("//div[@class='programme__body']");
        
        bool startListing = false;
        int lineCount = 0;
        string time = "";
        string actualStartTime = "";        // The time of the first show found at or after the supplied time

        foreach (var show in shows)
        {
            time = show.SelectSingleNode(".//a/@aria-label").GetAttributeValue("aria-label", "").Trim();
            // Example format = '1 Jun 11:30: Simply Nigella, Episode 6'
            var temp = time.Split(' ')[2].Split(':');
            time = temp[0] + ':' + temp[1];
            
            int comboTime = Convert.ToInt32(FormatDisplayTime(time));
            
            if(((!exactMatch && comboTime >= startTime) || (exactMatch && comboTime == startTime))
                && actualStartTime == String.Empty)
            {
                startListing = true;
                actualStartTime = time;
            }            

            if(startListing)
            {
                string title = show.SelectSingleNode(".//span[contains(@class, 'programme__title')]")?.InnerText.Trim().ToUpper();
                var titleLines = Utility.ParseParagraph(title, 34, 34, false);
                
                string body = show.SelectSingleNode(".//p[contains(@class, 'programme__synopsis')]/span")?.InnerText.Trim();
                var bodyLines = Utility.ParseParagraph(body, 34, 34, false);
        
                // Don't show a description for news or weather
                if(OnlyShowHeadline(title))
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
                sb.AppendLine($"{FormatDisplayTime(time)} </span><span class=\"ink{(int)Mode7Colour.White}\">{titleLines[0]}</span>");
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
        for(int i=0; i<18-lineCount; i++)  
        {
            sb.Append("<br>");  
        }
        sb.Append($"<p><span class=\"paper{(int)Mode7Colour.Blue} ink{(int)Mode7Colour.Yellow}\">&nbsp;&nbsp;More from CEEFAX in a moment >>>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>");

        // Only now do we now the true timespan. The end time shown is from the show that couldn't fit on this page.
        pageSb.Append(sb.Replace("{TimeSpan}", $"{FormatDisplayTime(actualStartTime)}-{FormatDisplayTime(time)}"));
        return Convert.ToInt32($"{FormatDisplayTime(time)}");
    }

    
    #endregion
}
