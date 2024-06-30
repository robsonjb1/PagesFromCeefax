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

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]UK MARKETS");
            sb.Append(OutputMarket("FTSE 100"));
            sb.Append(OutputMarket("FTSE 250"));
            sb.LineBreak(TeletextControl.AlphaRed);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]EUROPE MARKETS");
            sb.Append(OutputMarket("AEX"));
            sb.Append(OutputMarket("DAX"));
            sb.LineBreak(TeletextControl.AlphaRed);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]US MARKETS");
            sb.Append(OutputMarket("Dow Jones"));
            sb.Append(OutputMarket("Nasdaq"));
            sb.LineBreak(TeletextControl.AlphaRed);
           
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]ASIA MARKETS");
            sb.Append(OutputMarket("Hang Seng"));
            sb.Append(OutputMarket("Nikkei 225"));
            sb.LineBreak(TeletextControl.AlphaRed);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]CURRENCIES");
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
            TeletextControl colour = mr.Movement.StartsWith('-') ? TeletextControl.AlphaRed : TeletextControl.AlphaGreen;
            string partMovement = $"[{colour}] {mr.Movement}";
            string partClosed = $"[{TeletextControl.AlphaCyan}]{(mr.Closed ? " Closed" : "")}";

            sb.AppendLine($"[{TeletextControl.AlphaWhite}]{mr.Name.PadRightWithTrunc(14)}{mr.Value.PadLeftWithTrunc(9)}{partMovement}{partClosed}");
        }

        return sb;
    }

    private StringBuilder OutputCurrency(string currency)
    {
        StringBuilder sb = new();
        MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name.StartsWith("/" + currency));

        if (mr != null)
        {
            TeletextControl colour = mr.Movement.StartsWith('-') ? TeletextControl.AlphaRed : TeletextControl.AlphaGreen;
            string partMovement = $"[{colour}] {mr.Movement}";
            
            sb.AppendLine($"[{TeletextControl.AlphaWhite}]{currency.PadRightWithTrunc(14)}{mr.Value.PadLeftWithTrunc(9)}{partMovement}");
        }

        return sb;
    }

    #endregion
}