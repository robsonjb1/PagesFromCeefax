using System.Text.Json;
using API.Architecture;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public record MarketRecord
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Movement { get; set; }
    public bool MarketClosed { get; set;}
}

public interface IMarketData
{
    public List<MarketRecord> Markets { get; set; }
    public HLCurrencies Currencies { get; set; }
    public bool IsValid { get; set; }
}

public class MarketData : IMarketData
{
    public List<MarketRecord> Markets { get; set; } = new();
    public HLCurrencies Currencies { get; set; } = new();
    public bool IsValid { get; set; } = false;

    public MarketData(ICeefaxContent cc) { GetMarketData(cc); }
    
    private void GetMarketData(ICeefaxContent cc)
    {
        try
        {
            string html = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.Markets).Feed).ContentString;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Markets
            var markets = doc.DocumentNode.SelectNodes("//div[contains(@class, 'markets-table-wrap')]//tr");
            foreach (var market in markets)
            {
                string name = market.GetAttributeValue("data-symbol", "").Replace("&amp;", "&");
                string value = market.SelectSingleNode($".//td[2]/span")?.InnerText.Trim();
                string movement = market.SelectSingleNode($".//td[5]/span")?.InnerText.Trim();
                
                // Get the market close from Yahoo
                bool marketClosed = true;
                if(cc.UriCache.Exists(z => z.Tag == $"YH-{name}"))
                {
                    if(name == "MCX")
                    {        
                        marketClosed = cc.UriCache.FirstOrDefault(l => l.Tag == $"YH-UKX").ContentString.Contains(">At close:");
                    }
                    else
                    {
                        marketClosed = cc.UriCache.FirstOrDefault(l => l.Tag == $"YH-{name}").ContentString.Contains(">At close:");
                    }
                }

                if (name != String.Empty)
                {
                    Markets.Add(new MarketRecord()
                    {
                        Name = name,
                        Movement = movement,
                        Value = value,
                        MarketClosed = marketClosed
                    });
                }
            }
  
            // UK 10-year bond yields
            html = cc.UriCache.FirstOrDefault(l => l.Tag == "UK-10YRBOND").ContentString;
            doc.LoadHtml(html);
            string yield = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-5xl/9')]").InnerText;
            double change = Math.Round(Convert.ToDouble(doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'md:text-xl/7')]/span").InnerText), 2);
            string changeText = (change > 0) ? $"+{change:0.00}%" : $"{change:0.00}%";

            Markets.Add(new MarketRecord()
            {
                Name = "UK-10YRBOND",
                Movement = changeText,
                Value = yield,
                MarketClosed = false
            });

            // Currencies
            string json = cc.UriCache.First(l => l.Tag == "HL-Currencies").ContentString;
            Currencies = JsonSerializer.Deserialize<HLCurrencies>(json);

            IsValid = true;
        }
        catch(Exception ex)
        {
            Log.Fatal($"MARKETDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }

}
