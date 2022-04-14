using HtmlAgilityPack;

namespace PagesFromCeefax
{
    public class WeatherData
    {
        public readonly string TodayTitle;
        public readonly string TodayText;

        public readonly string TomorrowTitle;
        public readonly string TomorrowText;

        public readonly string OutlookTitle;
        public readonly string OutlookText;

        public WeatherData(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            TodayTitle = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[1]").InnerText;
            TomorrowTitle = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[2]").InnerText;
            OutlookTitle = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[3]").InnerText;

            TodayText = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[1]").NextSibling.InnerText;
            TomorrowText = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[2]").NextSibling.InnerText;
            OutlookText = doc.DocumentNode.SelectSingleNode("(//h4[@class='wr-c-text-forecast__summary-title gel-long-primer-bold gs-u-mt+'])[3]").NextSibling.InnerText;
        }
    }
}