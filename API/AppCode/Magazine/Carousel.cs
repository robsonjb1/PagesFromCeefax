using System;
using System.Text;
using PagesFromCeefax;

namespace API.AppCode.Magazine
{
    public interface ICarousel
    {
        public string BuildCarousel();
    }

    public class Carousel : ICarousel
    {
        public int MaxPages { get; set; } = 0;
        private StringBuilder DisplayHtml = new();

        public MagazineContent _content;
        public ITeletextPageWeather _tw;
        public ITeletextPageNews _tn;

        public Carousel(MagazineContent content, ITeletextPageNews tn, ITeletextPageWeather tw)
        {
            _content = content;
            _tw = tw;
            _tn = tn;
        }
        
        public string BuildCarousel()
        {
            // Construct DTO's
            var weather = new WeatherData(_content);

            // BUILD CAROUSEL

            // This is teletext
            BuildTeletextPage(Graphics.PromoTeletext);

            // News section
            BuildTeletextPage(Graphics.PromoNews);
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Home));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.World));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Politics));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Science));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Technology));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Sussex));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Business));

            // Sports section
            BuildTeletextPage(Graphics.PromoSport);
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Football));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Rugby));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Cricket));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Tennis));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Golf));
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Formula1));

            // Weather section
            BuildTeletextPage(Graphics.PromoWeather);
            if (weather.TempsValid)
            {
                BuildTeletextPage(_tw.CreateWeatherMap(weather));
            }
            BuildTeletextPage(_tw.CreateWeather(2, weather.TodayTitle, weather.TodayText));
            BuildTeletextPage(_tw.CreateWeather(3, weather.TomorrowTitle, weather.TomorrowText));
            BuildTeletextPage(_tw.CreateWeather(4, weather.OutlookTitle, weather.OutlookText));

            // Entertainment section
            BuildTeletextPage(Graphics.PromoTV);
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Entertainment));

            // Close
            BuildTeletextPage(Graphics.PromoLinks);

            // Insert stats
            DisplayHtml.AppendLine("<!-- The service started on: {PFC_SERVICESTART} and has built a total of {PFC_TOTALCAROUSELS} carousel(s). -->");
            DisplayHtml.AppendLine("<!-- The latest carousel is: {PFC_TIMESTAMP} taking {PFC_BUILDTIME}ms to build. -->");
            DisplayHtml.AppendLine("<!-- The service has handled the following requests -->");
            DisplayHtml.AppendLine("{PFC_TOTALREQUESTS}");

            // The number of total pages is required javascript page cycler
            DisplayHtml.AppendLine($"<div id='totalPages' style='display:none'>{MaxPages}</div>");

            return DisplayHtml.ToString();
        }

        private void BuildTeletextPage(StringBuilder newPage)
        {
            MaxPages++;

            // Generate the <div> enclosure that contains the individual page
            StringBuilder sb = new StringBuilder();
            DisplayHtml.AppendLine($"<div id=\"page{MaxPages}\" style=\"display:none\">");
            DisplayHtml.Append(newPage);
            DisplayHtml.Append("</div>");
        }

        private void BuildTeletextPage(List<StringBuilder> newPages)
        {
            foreach (StringBuilder newPage in newPages)
            {
                MaxPages++;

                // Generate the <div> enclosure that contains the individual page
                StringBuilder sb = new StringBuilder();
                DisplayHtml.AppendLine($"<div id=\"page{MaxPages}\" style=\"display:none\">");
                DisplayHtml.Append(newPage);
                DisplayHtml.Append("</div>");
            }
        }
    }
}

