using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public interface ITeletextPageMarkets
{
    public StringBuilder CreateMarketsPage();
}

public class TeletextPageMarkets : ITeletextPageMarkets
{
    private readonly ICeefaxContent _cc;
    private readonly IMarketData _md;
    
    public TeletextPageMarkets(ICeefaxContent cc, IMarketData md)
    {
        _cc = cc;
        _md = md;
    }

    #region Public Methods

   
    public StringBuilder CreateMarketsPage()
    {
        StringBuilder sb = new();
        if(_md.IsValid)             // Only construct the page if we have valid data
        {
            sb.Append(Graphics.HeaderMarkets);

            sb.AppendLineColour("UK MARKETS", Mode7Colour.Yellow);
            sb.Append(OutputMarket("FTSE 100"));
            sb.Append(OutputMarket("FTSE 250"));
            sb.LineBreak(Mode7Colour.Red);

            sb.AppendLineColour("EUROPE MARKETS", Mode7Colour.Yellow);
            sb.Append(OutputMarket("AEX"));
            sb.Append(OutputMarket("DAX"));
            sb.LineBreak(Mode7Colour.Red);
           
            sb.AppendLineColour("US MARKETS", Mode7Colour.Yellow);
            sb.Append(OutputMarket("Dow Jones"));
            sb.Append(OutputMarket("Nasdaq"));
            sb.LineBreak(Mode7Colour.Red);
           
            sb.AppendLineColour("ASIA MARKETS", Mode7Colour.Yellow);
            sb.Append(OutputMarket("Hang Seng"));
            sb.Append(OutputMarket("Nikkei 225"));
            sb.LineBreak(Mode7Colour.Red);
           
            sb.AppendLineColour("CURRENCIES", Mode7Colour.Yellow);
            sb.Append(OutputCurrency("EUR"));
            sb.Append(OutputCurrency("USD"));

            // Display footer
            sb.FooterText(_cc.Sections.Find(z => z.Name == CeefaxSectionType.Markets));
        }

        return sb;
    }
    #endregion

    #region Private Methods
    private StringBuilder OutputMarket(string marketName)
    {
        StringBuilder sb = new();
        MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name == marketName);

        if(mr != null)
        {
            string partMovement = Utility.LineColourFragment($"&nbsp;&nbsp;{mr.Movement}", 
                mr.Movement.StartsWith('-') ? Mode7Colour.Red : Mode7Colour.Green);

            string partClosed = Utility.LineColourFragment(mr.Closed ? "&nbsp;&nbsp;Closed" : "", Mode7Colour.Cyan);

            sb.AppendLineColour($"{mr.Name.PadHtmlLeft(14)}{mr.Value.PadHtmlRight(9)}{partMovement}{partClosed}", Mode7Colour.White);
        }

        return sb;
    }

    private StringBuilder OutputCurrency(string currency)
    {
        StringBuilder sb = new();
        MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name.StartsWith("/" + currency));

        if (mr != null)
        {
            string partMovement = Utility.LineColourFragment($"&nbsp;&nbsp;{mr.Movement}", 
                mr.Movement.StartsWith('-') ? Mode7Colour.Red : Mode7Colour.Green);

            sb.AppendLineColour($"{currency.PadHtmlLeft(14)}{mr.Value.PadHtmlRight(9)}{partMovement}", Mode7Colour.White);
        }

        return sb;
    }

    #endregion
}