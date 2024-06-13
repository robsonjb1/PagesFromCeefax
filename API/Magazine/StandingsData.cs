using API.Architecture;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public record StandingRecord
{
    public string Name { get; set; }
    public int Wins { get; set; }
    public int Points { get; set; }
}

public interface IStandingData
{
    public List<StandingRecord> Drivers { get; set; }
    public List<StandingRecord> Teams { get; set; }
    public bool IsValid { get; set; }
}

public class StandingsData : IStandingData
{
    public List<StandingRecord> Drivers { get; set; } = new();
    public List<StandingRecord> Teams { get; set; } = new();
    public bool IsValid { get; set; } = false;

    private readonly ICeefaxContent _cc;

    public StandingsData(ICeefaxContent cc)
    {
        _cc = cc;

        try
        {
            ParseSharePage(CeefaxSectionType.Standings);
            
            IsValid = true;
        }
        catch (Exception ex)
        {
            Log.Fatal($"STANDINGDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }

    private void ParseSharePage(CeefaxSectionType section)
    {
        string html = _cc.UriCache.First(l => l.Location == _cc.Sections.First(z => z.Name == section).Feed).ContentString;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
    }
}