using System.Text;
using API.Architecture;
using API.Magazine;
using HtmlAgilityPack;

namespace API.PageGenerators;

public interface ITeletextPageMarkets
{
    public StringBuilder CreateMarketsPage();
}

public class TeletextPageMarkets : ITeletextPageMarkets
{
    private readonly ICeefaxContent _mc;
    private readonly MarketData _md;
    
    public TeletextPageMarkets(ICeefaxContent mc)
    {
        _mc = mc;
        _md = GetMarketData();
    }

    #region Public Methods

    private MarketData GetMarketData()
    {
        string html = _mc.UrlCache.First(l => l.Location == _mc.Sections.First(z => z.Name == CeefaxSectionType.Markets).Feed).Content;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        MarketData md = new();
        var markets = doc.DocumentNode.SelectNodes("//tr[@class='ssrcss-xw0taf-EntryRow eohkjht10']");

        foreach (var market in markets)
        {
            string name = market.SelectSingleNode(".//div[@class='ssrcss-14tpdky-EntryName eohkjht9']")?.InnerText.Trim();
            string movement = market.SelectSingleNode("(.//div[@class='ssrcss-gastmb-InnerCell eohkjht0'])[1]")?.InnerText.Trim();
            string value = market.SelectSingleNode("(.//div[@class='ssrcss-gastmb-InnerCell eohkjht0'])[2]")?.InnerText.Trim();
            if(market.SelectSingleNode("(.//div[@class='ssrcss-gastmb-InnerCell eohkjht0'])[2]/span[1]") != null)
            {
                value = market.SelectSingleNode("(.//div[@class='ssrcss-gastmb-InnerCell eohkjht0'])[2]/span[1]")?.InnerText.Trim();
            }

				if (double.TryParse(value, out double n))
				{
					value = n.ToString("#,##0.00");
				}

            bool closed = market.SelectSingleNode(".//span[@class='ssrcss-12gx7m0-MarketStatus eohkjht1']")?.InnerText.Trim().ToUpper() == "CLOSED";
            
            if (name != null)
            {
                md.Markets.Add(new MarketRecord()
                {
                    Name = name,
                    Movement = (movement.StartsWith("0") ? String.Concat("=", movement) : movement.Replace("−", "-")),
                    Value = value.Replace("&euro;", "€"),
                    Closed = closed
                });
            }
        }

        return md;
    }

    public StringBuilder CreateMarketsPage()
    {
        StringBuilder sb = new();
        sb.Append(Graphics.HeaderMarkets);

        sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Yellow}\">UK MARKETS</span></p>");
        sb.Append(OutputMarket("FTSE 100"));
        sb.Append(OutputMarket("FTSE 250"));
        sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Red}\">=======================================</span></p>");

        sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Yellow}\">EUROPE MARKETS</span></p>");
        sb.Append(OutputMarket("AEX"));
        sb.Append(OutputMarket("DAX"));
        sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Red}\">=======================================</span></p>");

        sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Yellow}\">US MARKETS</span></p>");
        sb.Append(OutputMarket("Dow Jones"));
        sb.Append(OutputMarket("Nasdaq"));
        sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Red}\">=======================================</span></p>");

        sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Yellow}\">ASIA MARKETS</span></p>");
        sb.Append(OutputMarket("Hang Seng"));
        sb.Append(OutputMarket("Nikkei 225"));
        sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Red}\">=======================================</span></p>");

        sb.AppendLine($"<p><span class=\"indent ink{(int)Mode7Colour.Yellow}\">CURRENCIES</span></p>");
        sb.Append(OutputCurrency("EUR"));
        sb.Append(OutputCurrency("USD"));

        // Display footer
        Utility.FooterText(sb, _mc.Sections.Find(z => z.Name == CeefaxSectionType.Markets));

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
            sb.Append($"<p><span class=\"indent ink{(int)Mode7Colour.White}\">");
            sb.Append(mr.Name.PadRight(14, ' ').Replace(" ", "&nbsp;"));
            sb.Append(mr.Value.PadLeft(9, ' ').Replace(" ", "&nbsp;"));
            if (mr.Movement.StartsWith("-"))
            {
                sb.Append($"<span class=\"ink{(int)Mode7Colour.Red}\">");
            }
            else
            {
                sb.Append($"<span class=\"ink{(int)Mode7Colour.Green}\">");
            }
            sb.Append($"&nbsp;&nbsp;{mr.Movement}");
            sb.Append($"</span><span class=\"ink{(int)Mode7Colour.Cyan}\">");
            sb.Append(mr.Closed ? "&nbsp;&nbsp;Closed" : "");
            sb.Append("</span></p>");
        }

        return sb;
    }

    private StringBuilder OutputCurrency(string currency)
    {
        StringBuilder sb = new();
        MarketRecord mr = _md.Markets.FirstOrDefault(z => z.Name.StartsWith("/" + currency));

        if (mr != null)
        {
            sb.Append($"<p><span class=\"indent ink{(int)Mode7Colour.White}\">{currency.PadRight(16, ' ').Replace(" ", "&nbsp")}");
            sb.Append(mr.Value.PadRight(9, ' ').Replace(" ", "&nbsp;"));

            if (mr.Movement.StartsWith("-"))
            {
                sb.Append($"<span class=\"ink{(int)Mode7Colour.Red}\">");
            }
            else
            {
                sb.Append($"<span class=\"ink{(int)Mode7Colour.Green}\">");
            }
            sb.Append(mr.Movement);
            sb.Append("</span></p>");
        }

        return sb;
    }

    #endregion
}