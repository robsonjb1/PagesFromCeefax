using System.Text;
using API.Architecture;
using API.Magazine;
using HtmlAgilityPack;

namespace API.PageGenerators;

public interface ITeletextPageStanding
{
    public StringBuilder CreateStandingsPage();
}

public class TeletextPageStandings : ITeletextPageStanding
{
    private readonly ICeefaxContent _mc;
    private readonly IStandingData _sd;
    
    public TeletextPageStandings(ICeefaxContent mc, IStandingData sd)
    {
        _mc = mc;
        _sd = sd;
    }

    #region Public Methods

    public StringBuilder CreateStandingsPage()
    {
        StringBuilder sb = new StringBuilder();
        if(_sd.IsValid)             // Only construct the page if we have valid data
        {
            sb.Append(Graphics.HeaderFormula1);
        }
        return sb;
    }

    #endregion
}