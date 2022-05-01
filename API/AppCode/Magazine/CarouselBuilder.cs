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
                Content.UrlCache.First(l => l.Location == Content.Sections.First(z => z.Name == MagazineSectionType.Weather).Feed).Content!);

            // Initialise page generators
            TeletextPageNews tn = new TeletextPageNews(Content);
            TeletextPageWeather tw = new TeletextPageWeather(Content);

            // BUILD CAROUSEL

            // This is teletext
            tn.BuildTeletextPage(Graphics.PromoTeletext);

            // News section
            tn.BuildTeletextPage(Graphics.PromoNews);
            tn.CreateNewsSection(MagazineSectionType.Home);
            tn.CreateNewsSection(MagazineSectionType.World);
            tn.CreateNewsSection(MagazineSectionType.Politics);
            tn.CreateNewsSection(MagazineSectionType.Science);
            tn.CreateNewsSection(MagazineSectionType.Technology);
            tn.CreateNewsSection(MagazineSectionType.Sussex);
            tn.CreateNewsSection(MagazineSectionType.Business);
         
            // Sports section
            tn.BuildTeletextPage(Graphics.PromoSport);
            tn.CreateNewsSection(MagazineSectionType.Football);
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
          
            // Close
            tn.BuildTeletextPage(Graphics.PromoLinks);

            // Insert stats
            Content.DisplayHtml.AppendLine("<!-- The service started on: {PFC_SERVICESTART} and has built a total of {PFC_TOTALCAROUSELS} carousel(s). -->");
            Content.DisplayHtml.AppendLine("<!-- The latest carousel is: {PFC_TIMESTAMP} taking {PFC_BUILDTIME}ms to build. -->");
            Content.DisplayHtml.AppendLine("<!-- The service has handled the following requests -->");
            Content.DisplayHtml.AppendLine("{PFC_TOTALREQUESTS}");
        
            // The number of total pages is required javascript page cycler
            Content.DisplayHtml.AppendLine($"<div id='totalPages' style='display:none'>{Content.MaxPages}</div>");
        }
    }
}