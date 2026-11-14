using System.Text;
using API.Architecture;
using API.Magazine;
using Microsoft.ApplicationInsights.Extensibility;

namespace API.PageGenerators;


public class TeletextPageStandings(CeefaxContent mc, StandingsData sd)
{
    private readonly CeefaxContent _cc = mc;
    private readonly StandingsData _sd = sd;

    #region Public Methods

    public StringBuilder CreateFootballStandingsPage()
    {
        StringBuilder sb = new StringBuilder();
        CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.FootballStandings);

        if(_sd.IsValid)             // Only construct the page if we have valid data
        {
            sb.Append(Graphics.HeaderFootball);
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]PREMIER LEAGUE           [{TeletextControl.AlphaGreen}]P  W  L  D  [{TeletextControl.DownArrow}]");
            for(int i=0; i<20; i++)
            {
                // Skip the middle rows as they can't fit
                if ((i != 10) && (i != 11) && (i != 12))
                {
                    TeletextControl col = (i % 2 == 0) ? TeletextControl.AlphaWhite : TeletextControl.AlphaCyan;
                    sb.Append($"[{col}]{_sd.PremierLeague[i].Position.PadLeftWithTrunc(2)} {_sd.PremierLeague[i].Team.Replace(" & ", " ").PadRightWithTrunc(21)}");
                    sb.AppendLine($"{_sd.PremierLeague[i].Played.PadLeftWithTrunc(3)}{_sd.PremierLeague[i].Won.PadLeftWithTrunc(3)}{_sd.PremierLeague[i].Lost.PadLeftWithTrunc(3)}{_sd.PremierLeague[i].Drawn.PadLeftWithTrunc(3)}{_sd.PremierLeague[i].Points.PadLeftWithTrunc(3)}");
                }
                else
                {
                    if (i == 10) {
                        sb.LineBreak(TeletextControl.AlphaBlue);
                    }
                }
            }
            sb.FooterText(section, true);
        }
        return sb;
    }

    public StringBuilder CreateF1StandingsPage()
    {
        StringBuilder sb = new StringBuilder();
        CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.Motorsport);
        
        if(_sd.IsValid)             // Only construct the page if we have valid data
        {
            int count = 0;
            sb.Append(Graphics.HeaderFormula1);
            string driverHeading = $"DRIVER STANDINGS (ROUND {_sd.Drivers.Sum(z => z.Wins)})";
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]{driverHeading.PadRightWithTrunc(30)}[{TeletextControl.AlphaGreen}]wins pts");
            
            // Drivers
            foreach(var d in _sd.Drivers.Take(8))
            {
                TeletextControl col = (count % 2 == 0) ? TeletextControl.AlphaWhite : TeletextControl.AlphaCyan;
                sb.AppendLine($"[{col}]{d.Name.PadRightWithTrunc(16)} {d.Team.PadRightWithTrunc(16)}{d.Wins.PadLeftWithTrunc(2)}  {d.Points.PadLeftWithTrunc(3)}");
                
                count++;
            }
            sb.LineBreak(TeletextControl.AlphaBlue);
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]{"CONSTRUCTOR STANDINGS".PadRightWithTrunc(30)}[{TeletextControl.AlphaGreen}]wins pts");

            // Constructors
            count=0;
            foreach(var d in _sd.Constructors.Take(8))
            {
                TeletextControl col = (count % 2 == 0) ? TeletextControl.AlphaWhite : TeletextControl.AlphaCyan;
                sb.AppendLine($"[{col}]{d.Team.PadRightWithTrunc(33)}{d.Wins.PadLeftWithTrunc(2)} {d.Points.PadLeftWithTrunc(3)}");

                count++;
            }
            sb.FooterText(section);
        }

        return sb;
    }

    #endregion
}