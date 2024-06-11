using System.Text;
using API.Architecture;
using API.Magazine;
using HtmlAgilityPack;

namespace API.PageGenerators;

public interface ITeletextPageShares
{
    public StringBuilder CreateSharesPage();
}

public class TeletextPageShares : ITeletextPageShares
{
    private readonly ICeefaxContent _mc;
    private readonly IShareData _sd;
    
    public TeletextPageShares(ICeefaxContent mc, IShareData sd)
    {
        _mc = mc;
        _sd = sd;
    }

    #region Public Methods

    public StringBuilder CreateSharesPage()
    {
        StringBuilder sb = new StringBuilder();
        return sb;
    }

    #endregion
}