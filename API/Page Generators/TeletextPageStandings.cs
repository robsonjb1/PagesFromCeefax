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
            sb.AppendLineColour($"{driverHeading.PadHtmlLeft(28)}{Utility.LineColourFragment("wins points", Mode7Colour.Green)}", Mode7Colour.Yellow);
            
            // Drivers
            foreach(var d in _sd.Drivers.Take(8))
            {
                sb.AppendLineColour($"{d.Name.PadHtmlLeft(17)} {d.Team.PadHtmlLeft(12)}{d.Wins.PadHtmlRight(2)} {d.Points.PadHtmlRight(6)}",
                    (count % 2 == 0) ? Mode7Colour.White : Mode7Colour.Cyan);

                count++;
            }
            sb.LineBreak(Mode7Colour.Blue);
            sb.AppendLineColour($"{"CONSTRUCTOR STANDINGS".PadHtmlLeft(28)}{Utility.LineColourFragment("wins points", Mode7Colour.Green)}", Mode7Colour.Yellow);

            // Constructors
            count=0;
            foreach(var d in _sd.Constructors.Take(8))
            {
                sb.AppendLineColour($"{d.Team.PadHtmlLeft(30)}{d.Wins.PadHtmlRight(2)} {d.Points.PadHtmlRight(6)}",
                    (count % 2 == 0) ? Mode7Colour.White : Mode7Colour.Cyan);

                count++;
            }
            sb.FooterText(section);
        }

        return sb;
    }

    #endregion
}