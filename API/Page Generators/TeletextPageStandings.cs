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
            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">{driverHeading.PadHtmlLeft(27)}</span>");
            sb.AppendLine($"<span class=\"ink{(int)Mode7Colour.Green} indent\">wins points</span></p>");
            foreach(var d in _sd.Drivers.Take(8))
            {
                if (count % 2 == 0)
                { sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.White}\">"); }
                else
                { sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Cyan}\">"); } 
      
                sb.Append($"{d.Name.PadHtmlLeft(17)} {d.Team.PadHtmlLeft(12)}{d.Wins.PadHtmlRight(2)} {d.Points.PadHtmlRight(6)}</p>");
                count++;
            }
            sb.AppendLine("<br>");
            sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Yellow} indent\">CONSTRUCTOR STANDINGS</p>");
            
            count=0;
            foreach(var d in _sd.Constructors.Take(8))
            {
                if (count % 2 == 0)
                { sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.White}\">"); }
                else
                { sb.AppendLine($"<p><span class=\"ink{(int)Mode7Colour.Cyan}\">"); } 

                sb.AppendLine($"{d.Team.PadHtmlLeft(30)}{d.Wins.PadHtmlRight(2)} {d.Points.PadHtmlRight(6)}</p>");
                count++;
            }
            Utility.FooterText(sb, section);
        }

        return sb;
    }

    #endregion
}