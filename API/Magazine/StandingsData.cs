using API.Architecture;
using HtmlAgilityPack;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Serilog;

namespace API.Magazine;

public record StandingRecord
{
    public string Name { get; set; }
    public string Team { get; set;}
    public int Wins { get; set; }
    public int Points { get; set; }
}

public interface IStandingData
{
    public List<StandingRecord> Drivers { get; set; }
    public List<StandingRecord> Constructors { get; set; }
    public bool IsValid { get; set; }
}

public class StandingsData : IStandingData
{
    public List<StandingRecord> Drivers { get; set; } = new();
    public List<StandingRecord> Constructors { get; set; } = new();
    public bool IsValid { get; set; } = false;

    private readonly ICeefaxContent _cc;

    public StandingsData(ICeefaxContent cc)
    {
        _cc = cc;

        try
        {
            ParseStandingsPage(CeefaxSectionType.Standings);
            
            IsValid = true;
        }
        catch (Exception ex)
        {
            Log.Fatal($"STANDINGSDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }

    private void ParseStandingsPage(CeefaxSectionType section)
    {
        string html = _cc.UriCache.First(l => l.Location == _cc.Sections.First(z => z.Name == section).Feed).ContentString;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Drivers
        var drivers = doc.DocumentNode.SelectNodes("//section[@id='Drivers']//tr[@class='ssrcss-dhlz6k-TableRowBody e1icz100']");
        foreach(var driver in drivers)
        {
            var nodes = driver.SelectNodes(".//span");
            string driverName = nodes[2].InnerText;
            string team = nodes[4].InnerText;
            int wins = Convert.ToInt32(nodes[6].InnerText);
            int points = Convert.ToInt32(nodes[7].InnerText);
            
            Drivers.Add(new StandingRecord() { Name = driverName, Team = team, Points = points, Wins = wins });
        }

        // Constructors
        var constructors = doc.DocumentNode.SelectNodes("//section[@id='Constructors']//tr[@class='ssrcss-dhlz6k-TableRowBody e1icz100']");
        foreach(var constructor in constructors)
        {
            var nodes = constructor.SelectNodes(".//span");
            string team = nodes[2].InnerText;
            int wins = Convert.ToInt32(nodes[3].InnerText);
            int points = Convert.ToInt32(nodes[5].InnerText);

            Constructors.Add(new StandingRecord() { Team = team, Points = points, Wins = wins });
        }
    }
}