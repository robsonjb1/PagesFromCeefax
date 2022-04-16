using System;
using System.Text;

namespace PagesFromCeefax
{
    public class CarouselBuilder
    {
        public MagazineContent Content = new MagazineContent();

        public CarouselBuilder()
        {
            // Construct DTO's
            var weather = new WeatherData(
                Content.UrlCache.Find(l => l.Location == Content.Sections.Find(z => z.Name == MagazineSectionType.Weather)!.Feed)!.Content!);

            // Initialise page generators
            TeletextPageNews tn = new TeletextPageNews(Content);
            TeletextPageWeather tw = new TeletextPageWeather(Content);

            // BUILD CAROUSEL

            // This is teletext
            tn.BuildTeletextPage(Graphics.PromoTeletext);

            // News section
            tn.BuildTeletextPage(Graphics.PromoNews);
            tn.CreateNewsSection(MagazineSectionType.Home, true);   // true = display a default footer because we have a News in Brief following
            tn.CreateNewsInBrief(MagazineSectionType.Home);
            tn.CreateNewsSection(MagazineSectionType.World, true);
            tn.CreateNewsInBrief(MagazineSectionType.World);
            tn.CreateNewsSection(MagazineSectionType.Politics);
            tn.CreateNewsSection(MagazineSectionType.Science);
            tn.CreateNewsSection(MagazineSectionType.Technology);
            tn.CreateNewsSection(MagazineSectionType.Sussex);
            tn.CreateNewsSection(MagazineSectionType.Business);
            tn.CreateNewsInBrief(MagazineSectionType.Business);

            // Sports section
            tn.BuildTeletextPage(Graphics.PromoSport);
            tn.CreateNewsSection(MagazineSectionType.Football, true);
            tn.CreateNewsInBrief(MagazineSectionType.Football);
            tn.CreateNewsSection(MagazineSectionType.Rugby);
            tn.CreateNewsSection(MagazineSectionType.Cricket);
            tn.CreateNewsSection(MagazineSectionType.Tennis);
            tn.CreateNewsSection(MagazineSectionType.Golf);
            tn.CreateNewsSection(MagazineSectionType.Formula1);

            // Weather section
            tw.BuildTeletextPage(Graphics.PromoWeather);
            tw.CreateWeather(1, weather.TodayTitle, weather.TodayText);
            tw.CreateWeather(2, weather.TomorrowTitle, weather.TomorrowText);
            tw.CreateWeather(3, weather.OutlookTitle, weather.OutlookText);
            
            // Entertainment section
            tn.BuildTeletextPage(Graphics.PromoTV);
            tn.CreateNewsSection(MagazineSectionType.Entertainment);
            tn.CreateNewsInBrief(MagazineSectionType.Entertainment);

            // Close
            tn.BuildTeletextPage(Graphics.PromoLinks);

            // Insert stats
            Content.DisplayHtml.AppendLine("<!-- Service started: {PFC_SERVICESTART} -->");
            Content.DisplayHtml.AppendLine("<!-- Carousel built at: {PFC_TIMESTAMP} -->");
            Content.DisplayHtml.AppendLine("<!-- Total Requests: {PFC_TOTALREQUESTS} -->");
            Content.DisplayHtml.AppendLine("<!-- Total Carousels: {PFC_TOTALCAROUSELS} -->");

            // The number of total pages is required javascript page cycler
            Content.DisplayHtml.AppendLine($"<div id='totalPages' style='display:none'>{Content.MaxPages}</div>");
        }
    }
}