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

            var markets = doc.DocumentNode.SelectNodes("//tr[@class='nw-c-md-overview-table__row']");

            foreach (var market in markets)
            {
                string name = market.SelectSingleNode(".//span")?.InnerText.Trim();
                string movement = market.SelectSingleNode(".//div[@class='nw-c-md-overview-table__cell-inner']")?.InnerText.Trim();
                string value = market.SelectSingleNode("(.//div[@class='nw-c-md-overview-table__cell-inner'])[2]")?.InnerText.Trim();
                bool closed = market.SelectSingleNode(".//span[@class='nw-c-md-overview-markets__closed gel-minion']")?.InnerText.Trim() == "Closed";

                if (name != null)
                {
                    md.Markets.Add(new MarketRecord()
                    {
                        Name = name,
                        Movement = movement.Replace("&#45;", "-"),
                        Value = value.Replace("&euro;", "€"),
                        Closed = closed
                    });
                }
            }

            return md;
        }
    }
}