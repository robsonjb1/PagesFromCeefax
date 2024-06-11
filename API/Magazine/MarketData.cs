using API.Architecture;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public record MarketRecord
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Movement { get; set; }
    public bool Closed { get; set; }
}

public interface IMarketData
{
    public List<MarketRecord> Markets { get; set; }
    public bool IsValid { get; set; }
}

public class MarketData : IMarketData
{
    public List<MarketRecord> Markets { get; set; } = new();
    public bool IsValid { get; set; } = false;

    public MarketData(ICeefaxContent cc) { GetMarketData(cc); }
    
    private void GetMarketData(ICeefaxContent cc)
    {
        try
        {
            string html = cc.UriCache.First(l => l.Location == cc.Sections.First(z => z.Name == CeefaxSectionType.Markets).Feed).ContentString;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

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
                    Markets.Add(new MarketRecord()
                    {
                        Name = name,
                        Movement = movement.StartsWith("0") ? String.Concat("=", movement) : movement.Replace("−", "-"),
                        Value = value.Replace("&euro;", "€"),
                        Closed = closed
                    });
                }
            }

            IsValid = true;
        }
        catch(Exception ex)
        {
            Log.Fatal($"MARKETDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }
}