using System.Text;
using API.Architecture;
using API.Magazine;
using Microsoft.AspNetCore.Mvc.Filters;

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

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]UK MARKETS[{TeletextControl.AlphaWhite}]");
            sb.Append(OutputMarket("UKX", "FTSE 100"));
            sb.Append(OutputMarket("MCX", "FTSE 250"));
            sb.Append(OutputMarket("T1X", "techMARK"));
            sb.LineBreak(TeletextControl.AlphaRed);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]EUROPE MARKETS");
            sb.Append(OutputMarket("CAC", "CAC 40"));
            sb.Append(OutputMarket("DAX", "DAX"));
            sb.LineBreak(TeletextControl.AlphaRed);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]WORLDWIDE");
            sb.Append(OutputMarket("DJIA", "Dow Jones"));
            sb.Append(OutputMarket("COMP", "NASDAQ"));
            sb.Append(OutputMarket("HSI", "Hang Seng"));
            sb.Append(OutputMarket("NK225", "Nikkei 225"));

            sb.LineBreak(TeletextControl.AlphaRed);
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]CURRENCIES/BONDS");
            sb.Append(OutputCurrency("EUR"));
            sb.Append(OutputCurrency("USD"));
            sb.Append(OutputMarket("UK-10YRBOND", "UK 10-Year"));

            // Display footer
            sb.FooterText(_cc.Sections.Find(z => z.Name == CeefaxSectionType.Markets));
        }

        return sb;
    }
    #endregion

    #region Private Methods
    private StringBuilder OutputMarket(string marketName, string displayName)
    {
        StringBuilder sb = new();
        MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name == marketName);

        if(mr != null)
        {
            TeletextControl rateColour = mr.Movement.StartsWith('-') ? TeletextControl.AlphaRed : TeletextControl.AlphaGreen;

            if(marketName == "UK-10YRBOND")
            {
                // Inverse colour
                rateColour = rateColour == TeletextControl.AlphaRed ? TeletextControl.AlphaGreen : TeletextControl.AlphaRed;
            }

            string partMovement = $"[{rateColour}] {mr.Movement.PadLeftWithTrunc(7)}";
            string marketClosed = mr.MarketClosed ? "Closed" : "      ";

            sb.AppendLine($"[{TeletextControl.AlphaWhite}]{displayName.PadRightWithTrunc(10)} [{TeletextControl.AlphaCyan}]{marketClosed}[{TeletextControl.AlphaWhite}]{mr.Value.PadLeftWithTrunc(11)}{partMovement}");
        }

        return sb;
    }

    private StringBuilder OutputCurrency(string currency)
    {
        StringBuilder sb = new();
        var record = _md.Currencies.list.FirstOrDefault(z => z.FromISO == "GBP" && z.ToISO == currency);

        if (record != null)
        {
            double rate = Math.Round(Convert.ToDouble(record.RateCurrent), 4);
            double change = Math.Round(Convert.ToDouble(record.RateDayChangePercent), 2);

            TeletextControl rateColour = change < 0 ? TeletextControl.AlphaRed : TeletextControl.AlphaGreen;
            string changeFormatted = $"{change:0.00}";
            if(!changeFormatted.StartsWith("-"))
            {
                changeFormatted = "+" + changeFormatted;
            }
            string partMovement = $"[{rateColour}]  {changeFormatted}";
            
            sb.AppendLine($"[{TeletextControl.AlphaWhite}]GBP[{TeletextControl.RightArrow}]{currency.PadRightWithTrunc(17)}{rate.ToString("0.0000").PadLeftWithTrunc(9)}{partMovement:0.00}%");
        }

        return sb;
    }

    #endregion
}