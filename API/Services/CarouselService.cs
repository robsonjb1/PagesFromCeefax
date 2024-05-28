using System.Text;
using API.Architecture;
using API.PageGenerators;

namespace API.Services
{
    public interface ICarouselService
    {
        public string GetCarousel();
    }

    public class CarouselService : ICarouselService
    {
        public int MaxPages { get; set; } = 0;
        private readonly StringBuilder DisplayHtml = new();
       
        public ITeletextPageWeather _tw;
        public ITeletextPageNews _tn;
        public ITeletextPageMarkets _tm;
       
        public CarouselService(ITeletextPageNews tn, ITeletextPageWeather tw, ITeletextPageMarkets tm)
        {
            _tw = tw;
            _tn = tn;
            _tm = tm;
        }

        public string GetCarousel()
        {
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
            BuildTeletextPage(_tm.CreateMarketsPage());

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
            BuildTeletextPage(_tw.CreateWeatherMap());
            BuildTeletextPage(_tw.CreateWeatherPage(WeatherPage.Today));
            BuildTeletextPage(_tw.CreateWeatherPage(WeatherPage.Tomorrow));
            BuildTeletextPage(_tw.CreateWeatherPage(WeatherPage.Outlook));

            // Entertainment section
            BuildTeletextPage(Graphics.PromoTV);
            BuildTeletextPage(_tn.CreateNewsSection(MagazineSectionType.Entertainment));

            // Close
            BuildTeletextPage(Graphics.PromoLinks);

            // Insert stats
            DisplayHtml.AppendLine("<!-- The service started on: {PFC_SERVICESTART} and has built a total of {PFC_TOTALCAROUSELS} carousel(s). -->");
            DisplayHtml.AppendLine("<!-- The service has served {PFC_TOTALREQUESTS} request(s) since starting. -->");
            DisplayHtml.AppendLine("<!-- The latest carousel is: {PFC_TIMESTAMP} taking {PFC_BUILDTIME}ms to build. -->");
            
            // The number of total pages is required javascript page cycler
            DisplayHtml.AppendLine($"<div id='totalPages' style='display:none'>{MaxPages}</div>");

            return DisplayHtml.ToString();
        }

        #region Private Methods
        private void BuildTeletextPage(StringBuilder newPage)
        {
            MaxPages++;

            // Generate the <div> enclosure that contains the individual page
            DisplayHtml.AppendLine($"<div id=\"page{MaxPages}\" style=\"display:none\">");
            DisplayHtml.Append(newPage);

            DisplayHtml.Append("<div class=\"pageBreak\"><br><br><br></div>");

            DisplayHtml.Append("</div>");
        }

        private void BuildTeletextPage(List<StringBuilder> newPages)
        {
            newPages.ForEach(z => BuildTeletextPage(z));
        }
        #endregion
    }
}

