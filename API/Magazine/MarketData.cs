using System.Globalization;
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
            // Brent Crude Oil Price
            string brentCrude = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.MarketsOil).Feed).ContentString;
            var brentDoc = new HtmlDocument();
            brentDoc.LoadHtml(brentCrude);

            var oilPriceNode = brentDoc.DocumentNode.SelectSingleNode("//*[@id='market-current']//span[@data-field='value']/strong");
            var oilChangeNode = brentDoc.DocumentNode.SelectSingleNode("//*[@id='market-current']/div[2]/strong[2]");
            if (oilPriceNode is null || oilChangeNode is null)
            {
                throw new InvalidOperationException("Could not find the Brent crude price and change in the market data page.");
            }

            string oilPriceText = oilPriceNode.InnerText.Trim().Replace("$", "").Replace(",", "");
           
            string oilChangeText = oilChangeNode.SelectSingleNode("./span")?.InnerText.Trim() ?? oilChangeNode.InnerText.Trim();
            oilChangeText = oilChangeText.Trim('(', ')', '%');
            if (oilChangeNode.GetAttributeValue("class", "").Contains("negative", StringComparison.Ordinal))
            {
                oilChangeText = $"-{oilChangeText}%";
            }
            else
            {
                oilChangeText = $"+{oilChangeText}%";
            }

            Markets.Add(new MarketRecord()
            {
                Name = "BRENT-CRUDE",
                Movement = oilChangeText,
                Value = $"{oilPriceText}",
                MarketClosed = false
            });

            // Market Open/Close state
            string marketsOpen = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.MarketsOpen).Feed).ContentString;
            var marketsDoc = new HtmlDocument();
            marketsDoc.LoadHtml(marketsOpen);

            var marketExchanges = new Dictionary<string, string>
            {
                ["UKX"] = "LSE",
                ["MCX"] = "LSE",
                ["CAC"] = "EPA",
                ["DAX"] = "FSX",
                ["DJIA"] = "NYSE",
                ["COMP"] = "NewYork",
                ["HSI"] = "HKEX",
                ["NK225"] = "TSE"
            };
            var exchangeStatuses = new Dictionary<string, bool>();
            var exchangeRows = marketsDoc.DocumentNode.SelectNodes("//table//tr");
            if (exchangeRows is not null)
            {
                foreach (var row in exchangeRows)
                {
                    var exchangeNode = row.SelectSingleNode("./td[1]/a");
                    var statusNode = row.SelectSingleNode("./td[2]");
                    if (exchangeNode is null || statusNode is null)
                    {
                        continue;
                    }

                    string exchangeName = exchangeNode.InnerText.Trim();
                    int exchangeCodeStart = exchangeName.LastIndexOf('(');
                    if (exchangeCodeStart < 0 || !exchangeName.EndsWith(')'))
                    {
                        continue;
                    }

                    string exchangeCode = exchangeName[(exchangeCodeStart + 1)..^1].Trim();
                    string status = statusNode.InnerText.Trim();
                    if (status is "Open" or "Closed")
                    {
                        exchangeStatuses.TryAdd(exchangeCode, status == "Closed");
                    }
                }
            }


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
                    bool marketClosed = false;
                    if (marketExchanges.TryGetValue(name, out var exchange) &&
                        !exchangeStatuses.TryGetValue(exchange, out marketClosed))
                    {
                        throw new InvalidOperationException($"No market status found for exchange '{exchange}' mapped from '{name}'.");
                    }

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
                Value = $"{yield}",
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
