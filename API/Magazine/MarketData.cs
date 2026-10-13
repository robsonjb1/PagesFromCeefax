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

public class MarketsData 
{
    public List<MarketRecord> Markets { get; set; } = new();
    public HLCurrencies Currencies { get; set; } = new();
    public bool IsValid { get; set; } = false;

    public MarketsData(CeefaxContent cc) { GetMarketData(cc); }
    
    private void GetMarketData(CeefaxContent cc)
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
              
                if (name != String.Empty)
                {
                    Markets.Add(new MarketRecord()
                    {
                        Name = name,
                        Movement = movement,
                        Value = value,
                        MarketClosed = false
                    });
                }
            }
  
            // UK 10-year bond yields
            html = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.MarketsBonds).Feed).ContentString;
            doc.LoadHtml(html);
            string yield = doc.DocumentNode.SelectSingleNode("(//span[contains(@class, 'mod-ui-data-list__value')])[1]").InnerText;
            string changeText = doc.DocumentNode.SelectSingleNode("(//span[contains(@class, 'mod-ui-data-list__value')])[2]").InnerText;
            if(changeText.IndexOf(' ') > 0)
            {
                changeText = changeText.Substring(changeText.IndexOf(' ')+2).Trim();
            }
            else
            {
                changeText = "+0.00";
            }

            if(!changeText.StartsWith('+') && !changeText.StartsWith('-'))
            {
                changeText = "+" + changeText;
            }

            Markets.Add(new MarketRecord()
            {
                Name = "UK-10YRBOND",
                Movement = changeText,
                Value = $"{yield}00",
                MarketClosed = false
            });
            
            // Currencies
            string json = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.MarketsCurrencies).Feed).ContentString;
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
