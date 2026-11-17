using System.Text;
using API.Architecture;
using API.Magazine;

namespace API.PageGenerators;

public interface ITeletextPageMarkets
{
    public StringBuilder CreateMarketsPage();
}

public class TeletextPageMarkets(CeefaxContent cc, MarketsData md) : ITeletextPageMarkets
{
    private readonly CeefaxContent _cc = cc;
    private readonly MarketsData _md = md;

    #region Public Methods

    public StringBuilder CreateMarketsPage()
    {
        CeefaxSection section = _cc.Sections.Find(z => z.Name == CeefaxSectionType.Markets);
        StringBuilder sb = new();
        
        if(_md.IsValid)             // Only construct the page if we have valid data
        {
            sb.Append(section.Header);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]UK MARKETS[{TeletextControl.AlphaWhite}]");
            sb.Append(OutputMarket(TeletextControl.AlphaWhite, "UKX", "FTSE 100"));
            sb.Append(OutputMarket(TeletextControl.AlphaCyan, "MCX", "FTSE 250"));
            sb.LineBreak(TeletextControl.AlphaRed);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]EUROPE MARKETS");
            sb.Append(OutputMarket(TeletextControl.AlphaWhite, "CAC", "CAC 40"));
            sb.Append(OutputMarket(TeletextControl.AlphaCyan, "DAX", "DAX"));
            sb.LineBreak(TeletextControl.AlphaRed);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]WORLDWIDE");
            sb.Append(OutputMarket(TeletextControl.AlphaWhite, "DJIA", "Dow Jones"));
            sb.Append(OutputMarket(TeletextControl.AlphaCyan, "COMP", "NASDAQ"));
            sb.Append(OutputMarket(TeletextControl.AlphaWhite, "HSI", "Hang Seng"));
            sb.Append(OutputMarket(TeletextControl.AlphaCyan, "NK225", "Nikkei 225"));

            sb.LineBreak(TeletextControl.AlphaRed);
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]CURRENCIES/OTHER");
            sb.Append(OutputCurrency(TeletextControl.AlphaWhite, "EUR"));
            sb.Append(OutputCurrency(TeletextControl.AlphaCyan, "USD"));
			sb.Append(OutputMarket(TeletextControl.AlphaWhite, "UK-10YRBOND", "10-Year Bond"));
            sb.Append(OutputMarket(TeletextControl.AlphaCyan, "BRENT-CRUDE", "Brent Crude"));

            // Display footer
            sb.FooterText(section, true);
        }

        return sb;
    }
    #endregion

    #region Private Methods
    private StringBuilder OutputMarket(TeletextControl rowColour, string marketName, string displayName)
    {
        StringBuilder sb = new();
        MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name == marketName);

        if(mr != null)
        {
            TeletextControl rateColour = mr.Movement.StartsWith('-') ? TeletextControl.AlphaRed : TeletextControl.AlphaGreen;

            if(marketName == "UK-10YRBOND" || marketName == "BRENT-CRUDE")
            {
                // Inverse colour
                rateColour = rateColour == TeletextControl.AlphaRed ? TeletextControl.AlphaGreen : TeletextControl.AlphaRed;
            }

            string partMovement = $"[{rateColour}]{mr.Movement.PadLeftWithTrunc(7)}";
            string marketClosed = mr.MarketClosed ? " Closed " : "        ";

            sb.AppendLine($"[{rowColour}]{displayName.PadRightWithTrunc(12)}{marketClosed}{mr.Value.PadLeftWithTrunc(11)}{partMovement}");
        }

        return sb;
    }

    private StringBuilder OutputCurrency(TeletextControl rowColour, string currency)
    {
        StringBuilder sb = new();
        var record = _md.Currencies.list.FirstOrDefault(z => z.FromISO == "GBP" && z.ToISO == currency);

        if (record != null)
        {
            double rate = Math.Round(Convert.ToDouble(record.RateCurrent), 2);
            double change = Math.Round(Convert.ToDouble(record.RateDayChangePercent), 2);

            TeletextControl rateColour = change < 0 ? TeletextControl.AlphaRed : TeletextControl.AlphaGreen;
            string changeFormatted = $"{change:0.00}";
            if(!changeFormatted.StartsWith('+') && !changeFormatted.StartsWith('-'))
            {
                changeFormatted = "+" + changeFormatted;
            }
            string partMovement = $"[{rateColour}] {changeFormatted}";
            
            sb.AppendLine($"[{rowColour}]GBP[{TeletextControl.RightArrow}]{currency.PadRightWithTrunc(20)}{rate.ToString("0.00").PadLeftWithTrunc(7)}{partMovement:0.00}%");
        }

        return sb;
    }

    #endregion
}