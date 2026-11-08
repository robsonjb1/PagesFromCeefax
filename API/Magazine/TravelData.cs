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
    public List<TravelRecord> LGWArrivals { get; set; } = [];
    public DateTime LastRefreshUTC { get; set; }    
    public bool IsValid { get; set; } = false;

    private readonly CeefaxContent _cc;

    public TravelData(CeefaxContent cc)
    {
        _cc = cc;
        LastRefreshUTC = DateTime.UtcNow;

        try
        {
            SOGDepartures = ParseTrainTimes(CeefaxSectionType.TravelTrains);
            LGWArrivals = ParseFlightTimes(CeefaxSectionType.TravelFlights);
          
            IsValid = true;
        }
        catch (Exception ex)
        {
            Log.Fatal($"TRAVELDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }

    private List<TravelRecord> ParseFlightTimes(CeefaxSectionType section)
    {
        List<TravelRecord> flightTimes = new();
        string html = _cc.UriCache.First(l => l.Location == _cc.Sections.First(z => z.Name == section).Feed).ContentString;
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        HtmlNode table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'flights-table')]");
        HtmlNodeCollection headers = table?.SelectNodes(".//thead//th");
        if (headers is null)
        {
            return flightTimes;
        }

        var columnIndexes = headers
            .Select((header, index) => new
            {
                Name = GetText(header, "."),
                Index = index
            })
            .ToDictionary(column => column.Name, column => column.Index, StringComparer.OrdinalIgnoreCase);

        if (!columnIndexes.TryGetValue("Time", out int timeIndex) ||
            !columnIndexes.TryGetValue("From", out int destinationIndex) ||
            !columnIndexes.TryGetValue("Flight No", out int operatedByIndex) ||
            !columnIndexes.TryGetValue("Status", out int statusIndex) ||
            !columnIndexes.TryGetValue("Terminal", out int platformIndex))
        {
            return flightTimes;
        }

        var flights = table.SelectNodes(
            ".//tbody/tr[contains(concat(' ', normalize-space(@class), ' '), ' flight-line ')]");
        if (flights is null)
        {
            return flightTimes;
        }

        int lastRequiredColumnIndex = new[]
        {
            timeIndex,
            destinationIndex,
            operatedByIndex,
            statusIndex,
            platformIndex
        }.Max();

        foreach (HtmlNode flight in flights)
        {
            var cells = flight.SelectNodes(
                "./td[not(contains(concat(' ', normalize-space(@class), ' '), ' toggle '))]");
            if (cells is null || cells.Count <= lastRequiredColumnIndex)
            {
                continue;
            }

            string time = GetText(cells[timeIndex], ".");
            string destination = GetText(cells[destinationIndex], ".");
            if (string.IsNullOrEmpty(time) || string.IsNullOrEmpty(destination))
            {
                continue;
            }

            flightTimes.Add(new TravelRecord
            {
                Time = time.Replace(":", ""),
                Destination = destination,
                OperatedBy = GetText(cells[operatedByIndex], "."),
                Status = GetText(cells[statusIndex], "."),
                Platform = GetText(cells[platformIndex], ".")
            });
        }

        return flightTimes;
    }

    private List<TravelRecord> ParseTrainTimes(CeefaxSectionType section)
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
        return Utility.CleanHTML(HtmlEntity.DeEntitize(text).Trim());
    }
}