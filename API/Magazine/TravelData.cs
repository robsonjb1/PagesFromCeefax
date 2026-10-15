using HtmlAgilityPack;
using Serilog;
using API.Architecture;

namespace API.Magazine;

public record TravelRecord
{
    public string Time { get; set; }
    public string Destination { get; set;}
    public string Platform { get; set; }
    public string OperatedBy { get;set; }
    public string Status { get; set; }
   
}
public class TravelData
{
    public List<TravelRecord> SOGDepartures { get; set; } = [];
    public DateTime LastRefreshUTC { get; set; }    
    public bool IsValid { get; set; } = false;

    private readonly CeefaxContent _cc;

    public TravelData(CeefaxContent cc)
    {
        _cc = cc;
        LastRefreshUTC = DateTime.UtcNow;

        try
        {
            SOGDepartures = ParseTimes(CeefaxSectionType.Travel);
          
            IsValid = true;
        }
        catch (Exception ex)
        {
            Log.Fatal($"TRAVELDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }

    private List<TravelRecord> ParseTimes(CeefaxSectionType section)
    {
        List<TravelRecord> trainTimes = new();
        string html = _cc.UriCache.First(l => l.Location == _cc.Sections.First(z => z.Name == section).Feed).ContentString;
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        var services = doc.DocumentNode.SelectNodes("//ul[contains(@class, 'services')]/li[contains(@class, 'service')]");
        if (services is null)
        {
            return trainTimes;
        }

        foreach (var service in services)
        {
            string time = GetText(service, ".//span[contains(@class, 'scheduled-time')]");
            string destination = GetText(service, ".//span[contains(@class, 'destination')]");

            if (string.IsNullOrEmpty(time) || string.IsNullOrEmpty(destination))
            {
                continue;
            }

            string operatedBy = GetText(service, ".//span[contains(@class, 'operator')]");
            string platform = GetText(service, ".//span[contains(@class, 'platform')]");
            platform = platform.Replace("Plat.", "", StringComparison.OrdinalIgnoreCase).Trim();

            trainTimes.Add(new TravelRecord
            {
                Time = time.Replace(":", ""),
                Destination = destination,
                Platform = platform,
                OperatedBy = operatedBy,
                Status = GetText(service, ".//span[contains(@class, 'expected')]").Replace(":", "")
            });
        }

        return trainTimes;
    }

    private static string GetText(HtmlNode node, string xpath)
    {
        string text = node.SelectSingleNode(xpath)?.InnerText ?? string.Empty;
        return HtmlEntity.DeEntitize(text).Trim();
    }
}