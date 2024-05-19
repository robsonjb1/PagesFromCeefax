using System;
using System.Text.Json;
using API.Architecture;
using API.DataTransferObjects;
using API.Magazine;
using HtmlAgilityPack;

namespace API.Services
{
    public interface IMarketService
    {
        public MarketData GetMarketData();
    }

    public class MarketService : IMarketService
    {
        private readonly IMagazineContent _content;

        public MarketService(IMagazineContent content)
        {
            _content = content;
        }

        public MarketData GetMarketData()
        {
            string html = _content.UrlCache.First(l => l.Location == _content.Sections.First(z => z.Name == MagazineSectionType.Markets).Feed).Content;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            MarketData md = new();
            md.LastRefreshUTC = DateTime.UtcNow;

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

				if (int.TryParse(value, out int n))
				{
					value = n.ToString("#,##0.00")
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
    }
}