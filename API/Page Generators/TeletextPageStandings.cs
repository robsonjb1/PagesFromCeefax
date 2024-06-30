using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public interface ITeletextPageStanding
{
    public StringBuilder CreateStandingsPage();
}

public class TeletextPageStandings : ITeletextPageStanding
{
    private readonly ICeefaxContent _cc;
    private readonly IStandingData _sd;
    
    public TeletextPageStandings(ICeefaxContent mc, IStandingData sd)
    {
        _cc = mc;
        _sd = sd;
    }

    #region Public Methods

    public StringBuilder CreateStandingsPage()
    {
        StringBuilder sb = new StringBuilder();
        CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.Formula1);
        
        if(_sd.IsValid)             // Only construct the page if we have valid data
        {
            int count = 0;
            sb.Append(Graphics.HeaderFormula1);
            string driverHeading = $"DRIVER STANDINGS (ROUND {_sd.Drivers.Sum(z => z.Wins)})";
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]{driverHeading.PadRightWithTrunc(27)}[{TeletextControl.AlphaGreen}]wins points");
            
            // Drivers
            foreach(var d in _sd.Drivers.Take(8))
            {
                TeletextControl col = (count % 2 == 0) ? TeletextControl.AlphaWhite : TeletextControl.AlphaCyan;
                sb.AppendLine($"[{col}]{d.Name.PadRightWithTrunc(17)} {d.Team.PadRightWithTrunc(12)}{d.Wins.PadLeftWithTrunc(2)} {d.Points.PadLeftWithTrunc(6)}");
                
                count++;
            }
            sb.LineBreak(TeletextControl.AlphaBlue);
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]{"CONSTRUCTOR STANDINGS".PadRightWithTrunc(27)}[{TeletextControl.AlphaGreen}]wins points");

            // Constructors
            count=0;
            foreach(var d in _sd.Constructors.Take(8))
            {
                TeletextControl col = (count % 2 == 0) ? TeletextControl.AlphaWhite : TeletextControl.AlphaCyan;
                sb.AppendLine($"[{col}]{d.Team.PadRightWithTrunc(30)}{d.Wins.PadLeftWithTrunc(2)} {d.Points.PadLeftWithTrunc(6)}");

                count++;
            }
            sb.FooterText(section);
        }

        return sb;
    }

    #endregion
}