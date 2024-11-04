using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Xml;
using API.Architecture;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public record MarketRecord
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Movement { get; set; }
}

public interface IMarketData
{
    public List<MarketRecord> Markets { get; set; }
    public List<MarketRecord> Risers { get; set;}
    public List<MarketRecord> Fallers { get; set; }
    public HLCurrencies Currencies { get; set; }
    public bool IsValid { get; set; }
}

public class MarketData : IMarketData
{
    public List<MarketRecord> Markets { get; set; } = new();
    public List<MarketRecord> Risers { get; set; } = new();
    public List<MarketRecord> Fallers { get; set; } = new();
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
                
                if (name != String.Empty)
                {
                    Markets.Add(new MarketRecord()
                    {
                        Name = name,
                        Movement = movement,
                        Value = value
                    });
                }
            }

            // Risers
            Risers = ParseRiserFallerList(cc, "HLRisers");
            
            // Fallers
            Fallers = ParseRiserFallerList(cc, "HLFallers");
            
            // Currencies
            string json = cc.UriCache.First(l => l.Tag == "HLCurrencies").ContentString;
            Currencies = JsonSerializer.Deserialize<HLCurrencies>(json);

            IsValid = true;
        }
        catch(Exception ex)
        {
            Log.Fatal($"MARKETDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }

    private List<MarketRecord> ParseRiserFallerList(ICeefaxContent cc, string section)
    {
        List<MarketRecord> risersFallers = new();
        string html = cc.UriCache.First(l => l.Tag == section).ContentString;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var companies = doc.DocumentNode.SelectNodes("//table[@class='stockTable']/tbody//tr");
        if(companies != null) 
        { 
            foreach (var company in companies)
            {
                string name = company.SelectSingleNode($".//td[2]/a")?.InnerText.Trim().Replace("&amp;", "&");
                string value = company.SelectSingleNode($".//td[3]/span")?.InnerText.Trim();
                string movement = company.SelectSingleNode($".//td[5]/span")?.InnerText.Trim();

                if (name != String.Empty)
                {
                    risersFallers.Add(new MarketRecord()
                    {
                        Name = name,
                        Movement = movement,
                        Value = value
                    });
                }
            }
        }

        return risersFallers;
    }
}
