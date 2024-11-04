using System.Text;
using API.Architecture;
using API.Magazine;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.PageGenerators;

public interface ITeletextPageMarkets
{
    public StringBuilder CreateMarketsPage();
    public StringBuilder CreateRisersFallersPage();
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

   
    public StringBuilder CreateRisersFallersPage()
    {
        StringBuilder sb = new();
        if(_md.IsValid)             // Only construct the page if we have valid data
        {
            sb.Append(Graphics.HeaderShares);
        
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]FTSE 250: TOP RISERS[{TeletextControl.AlphaWhite}]");
            sb.Append(OutputRiserFallerList(_md.Risers.Take(8).ToList()));
            if(_md.Risers.Count == 0)
            {
                sb.AppendLine($"[{TeletextControl.AlphaWhite}]No share information available.");
                sb.PadLines(7);
            }
            else
            {   
                sb.PadLines(8 - _md.Risers.Count); 
            }
            sb.LineBreak(TeletextControl.AlphaRed);
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]FTSE 250: TOP FALLERS[{TeletextControl.AlphaWhite}]");
            sb.Append(OutputRiserFallerList(_md.Fallers.Take(8).OrderBy(z => z.Movement).ToList()));
            if(_md.Fallers.Count == 0)
            {
                sb.AppendLine($"[{TeletextControl.AlphaWhite}]No share information available.");
                sb.PadLines(7);
            }
            else
            {   
                sb.PadLines(8 - _md.Risers.Count); 
            }
                        
            // Display footer
            sb.FooterText(_cc.Sections.Find(z => z.Name == CeefaxSectionType.Markets));
        }

        return sb;
    }

    private StringBuilder OutputRiserFallerList(List<MarketRecord> companies)
    {
        StringBuilder sb = new();
        TeletextControl colour = TeletextControl.AlphaWhite;
        foreach(var company in companies)
        {
            TeletextControl rateColour = company.Movement.StartsWith('-') ? TeletextControl.AlphaRed : TeletextControl.AlphaGreen;
            string partMovement = $"[{rateColour}] {company.Movement.PadLeftWithTrunc(7)}";
          
            string abbreviatedName = company.Name;
            if(company.Name.Length > 30)
            {
                abbreviatedName = company.Name.Substring(0, 27) + "...";   
            }
            sb.AppendLine($"[{colour}]{abbreviatedName.PadRightWithTrunc(30)}{partMovement}");

            // Toggle line colour
            colour = colour == TeletextControl.AlphaWhite ? TeletextControl.AlphaCyan : TeletextControl.AlphaWhite;
        }

        return sb;
    }

    public StringBuilder CreateMarketsPage()
    {
        StringBuilder sb = new();
        if(_md.IsValid)             // Only construct the page if we have valid data
        {
            sb.Append(Graphics.HeaderMarkets);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]UK MARKETS[{TeletextControl.AlphaWhite}]");
            sb.Append(OutputMarket("UKX", "FTSE 100"));
            sb.Append(OutputMarket("MCX", "FTSE 250"));
            sb.LineBreak(TeletextControl.AlphaRed);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]EUROPE MARKETS");
            sb.Append(OutputMarket("CAC", "CAC 40"));
            sb.Append(OutputMarket("DAX", "DAX"));
            sb.LineBreak(TeletextControl.AlphaRed);

            sb.AppendLine($"[{TeletextControl.AlphaYellow}]US MARKETS");
            sb.Append(OutputMarket("DJIA", "Dow Jones"));
            sb.Append(OutputMarket("COMP", "NASDAQ"));
            sb.LineBreak(TeletextControl.AlphaRed);
           
            sb.AppendLine($"[{TeletextControl.AlphaYellow}]ASIA MARKETS");
            sb.Append(OutputMarket("HSI", "Hang Seng"));
            sb.Append(OutputMarket("NK225", "Nikkei 225"));

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
    private StringBuilder OutputMarket(string marketName, string displayName)
    {
        StringBuilder sb = new();
        MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name == marketName);

        if(mr != null)
        {
            TeletextControl rateColour = mr.Movement.StartsWith('-') ? TeletextControl.AlphaRed : TeletextControl.AlphaGreen;
            string partMovement = $"[{rateColour}] {mr.Movement.PadLeftWithTrunc(7)}";
          
            sb.AppendLine($"[{TeletextControl.AlphaWhite}]{displayName.PadRightWithTrunc(18)}{mr.Value.PadLeftWithTrunc(12)}{partMovement}");
        }

        return sb;
    }

    private StringBuilder OutputCurrency(string currency)
    {
        StringBuilder sb = new();
        var record = _md.Currencies.list.FirstOrDefault(z => z.FromISO == "GBP" && z.ToISO == currency);

        if (record != null)
        {
            double rate = Math.Round(Convert.ToDouble(record.RateCurrent), 2);
            double change = Math.Round(Convert.ToDouble(record.RateDayChangePercent), 2);

            TeletextControl rateColour = change < 0 ? TeletextControl.AlphaRed : TeletextControl.AlphaGreen;
            string partMovement = $"[{rateColour}]  {(change >=0 ? "+" : "")}{change}";
            
            sb.AppendLine($"[{TeletextControl.AlphaWhite}]GBP/{currency.PadRightWithTrunc(17)}{rate.ToString("#.#0").PadLeftWithTrunc(9)}{partMovement}%");
        }

        return sb;
    }

    #endregion
}