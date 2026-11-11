using System.Globalization;
using API.Architecture;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public record F1StandingsRecord
{
    public string Name { get; set; }
    public string Team { get; set;}
    public int Wins { get; set; }
    public int Points { get; set; }
}

public record FootballStandingsRecord
{
    public int Position {get; set; }
    public string Team { get; set;}
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int Points { get; set; }
    public int Played { get; set; }
}

public class StandingsData 
{
    public List<F1StandingsRecord> Drivers { get; set; } = [];
    public List<F1StandingsRecord> Constructors { get; set; } = [];
    public List<FootballStandingsRecord> PremierLeague { get; set; } = [];
    public bool IsValid { get; set; } = false;

    private readonly CeefaxContent _cc;

    public StandingsData(CeefaxContent cc)
    {
        _cc = cc;

        try
        {
            ParseF1StandingsPage(CeefaxSectionType.F1Standings);
            ParseFootballStandingsPage(CeefaxSectionType.FootballStandings);
            
            IsValid = true;
        }
        catch (Exception ex)
        {
            Log.Fatal($"STANDINGSDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }

    private void ParseFootballStandingsPage(CeefaxSectionType section)
    {
        string html = _cc.UriCache.First(l => l.Location == _cc.Sections.First(z => z.Name == section).Feed).ContentString;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var table = doc.DocumentNode.SelectSingleNode("//section[@id='PremierLeague']//table[@data-testid='football-table']");
        var rows = table?.SelectNodes(".//tbody/tr");
        if (rows is null || rows.Count == 0)
        {
            throw new InvalidDataException("The Premier League standings table was not found or contained no rows.");
        }

        foreach (var row in rows)
        {
            var teamCell = row.SelectSingleNode("./td[@aria-label='Team']");
            var positionNode = teamCell?.SelectSingleNode(".//span");
            var teamNode = teamCell?.SelectSingleNode(".//a");
            if (positionNode is null || teamNode is null)
            {
                throw new InvalidDataException("A Premier League standings row was missing its position or team.");
            }

            PremierLeague.Add(new FootballStandingsRecord
            {
                Position = ReadInteger(positionNode.InnerText, "position"),
                Team = HtmlEntity.DeEntitize(teamNode.InnerText).Trim(),
                Won = ReadCellInteger(row, "Won"),
                Drawn = ReadCellInteger(row, "Drawn"),
                Lost = ReadCellInteger(row, "Lost"),
                Points = ReadCellInteger(row, "Points"),
                Played = ReadCellInteger(row, "Played")
            });
        }

        static int ReadCellInteger(HtmlNode row, string label)
        {
            var cell = row.SelectSingleNode($"./td[@aria-label='{label}']");
            if (cell is null)
            {
                throw new InvalidDataException($"A Premier League standings row was missing its {label} value.");
            }

            return ReadInteger(cell.InnerText, label);
        }

        static int ReadInteger(string text, string label)
        {
            var valueText = HtmlEntity.DeEntitize(text).Trim();
            if (!int.TryParse(valueText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                throw new InvalidDataException($"Could not parse the Premier League standings {label}: '{valueText}'.");
            }

            return value;
        }
    }

    private void ParseF1StandingsPage(CeefaxSectionType section)
    {
        string html = _cc.UriCache.First(l => l.Location == _cc.Sections.First(z => z.Name == section).Feed).ContentString;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Drivers
        var drivers = doc.DocumentNode.SelectNodes("//section[@id='Drivers']//tr[@class='ssrcss-1if37cu-TableRowBody e1icz100']");
        foreach(var driver in drivers)
        {
            var nodes = driver.SelectNodes(".//span");
            string driverName = nodes[2].InnerText;
            //driverName = string.Join(" ", driverName.Split().Reverse().Take(2).Reverse()); // Take last two words of name only
        
            string team = nodes[4].InnerText;
            int wins = Convert.ToInt32(nodes[8].InnerText);
            int points = Convert.ToInt32(nodes[10].InnerText);
            
            Drivers.Add(new F1StandingsRecord() { Name = driverName, Team = team, Points = points, Wins = wins });
        }

        // Constructors
        var constructors = doc.DocumentNode.SelectNodes("//section[@id='Constructors']//tr[@class='ssrcss-1if37cu-TableRowBody e1icz100']");
        foreach(var constructor in constructors)
        {
            var nodes = constructor.SelectNodes(".//span");
            string team = nodes[2].InnerText;
            int wins = Convert.ToInt32(nodes[5].InnerText);
            int points = Convert.ToInt32(nodes[7].InnerText);

            Constructors.Add(new F1StandingsRecord() { Team = team, Points = points, Wins = wins });
        }
    }
}
