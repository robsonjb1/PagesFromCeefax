using API.Architecture;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public record TVListing
{
    public string StartTime { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

public record ChannelSchedule
{
    public string Day { get; set; }
    public CeefaxSectionType Channel { get; set; }
    public List<TVListing> Listings { get; set;} = new();

    public ChannelSchedule(CeefaxSectionType c)
    {
        Channel = c;
    }
}

public interface ITVListingData
{
    public List<ChannelSchedule> Schedules { get ; set; }
    public bool IsValid { get; set; }
}

public class TVListingData : ITVListingData
{
    public List<ChannelSchedule> Schedules { get ; set; } = new();
    public bool IsValid { get; set; } = false;
    private readonly ICeefaxContent _cc;

    public TVListingData(ICeefaxContent cc)
    {
        _cc = cc;

        AddListings(CeefaxSectionType.TVScheduleBBC1);
        AddListings(CeefaxSectionType.TVScheduleBBC2);
        AddListings(CeefaxSectionType.TVScheduleBBC4);
    }

    private void AddListings(CeefaxSectionType channel)
    {
        try
        {
            ChannelSchedule schedule = new ChannelSchedule(channel);
    
            string html = _cc.UriCache.First(l => l.Location == _cc.Sections.First(z => z.Name == channel).Feed).ContentString;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            string day = doc.DocumentNode.SelectSingleNode("//time").InnerText.Trim();
            schedule.Day = day[..day.IndexOf(" ")].ToUpper();

            var shows = doc.DocumentNode.SelectNodes("//div[@class='programme__body']");
       
            foreach (var show in shows)
            {
                TVListing listing = new();
                
                string time = show.SelectSingleNode(".//a/@aria-label").GetAttributeValue("aria-label", "").Trim();
                // Example format = '1 Jun 11:30: Simply Nigella, Episode 6'
                var temp = time.Split(' ')[2].Split(':');
                time = String.Concat(temp[0], ':', temp[1]);

                listing.StartTime = time;
                listing.Title = show.SelectSingleNode(".//span[contains(@class, 'programme__title')]")?.InnerText.Trim().ToUpper();
                listing.Description = show.SelectSingleNode(".//p[contains(@class, 'programme__synopsis')]/span")?.InnerText.Trim();
    
                schedule.Listings.Add(listing);
            }

            Schedules.Add(schedule);          
            IsValid = true;
        }
        catch(Exception ex)
        {
            Log.Fatal($"LISTINGDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }
}
