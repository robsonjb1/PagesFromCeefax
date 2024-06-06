using System.Text;
using API.Architecture;
using API.PageGenerators;
using Serilog;

namespace API.Services;

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
    public ITeletextPageSchedule _ts;
   
    public CarouselService(ITeletextPageNews tn, ITeletextPageWeather tw, ITeletextPageMarkets tm, ITeletextPageSchedule ts)
    {
        _tw = tw;
        _tn = tn;
        _tm = tm;
        _ts = ts;
    }

    public string GetCarousel()
    {
        try
        {
            // This is teletext
            BuildTeletextPage(Graphics.PromoTeletext);

            // News section
            BuildTeletextPage(Graphics.PromoNews);
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Home));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.World));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Politics));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Science));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Technology));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Sussex));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Business));
            BuildTeletextPage(_tm.CreateMarketsPage());

            // Sports section
            BuildTeletextPage(Graphics.PromoSport);
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Football));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Rugby));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Cricket));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Tennis));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Golf));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Formula1));

            // Weather section
            BuildTeletextPage(Graphics.PromoWeather);
            BuildTeletextPage(_tw.CreateWeatherMap());
            BuildTeletextPage(_tw.CreateWeatherPage(WeatherSubPage.Today));
            BuildTeletextPage(_tw.CreateWeatherPage(WeatherSubPage.Tomorrow));
            BuildTeletextPage(_tw.CreateWeatherPage(WeatherSubPage.Outlook));

            // Entertainment section
            BuildTeletextPage(Graphics.PromoTV);
            BuildTeletextPage(_ts.CreateSchedule(CeefaxSectionType.TVScheduleBBC1));
            BuildTeletextPage(_ts.CreateSchedule(CeefaxSectionType.TVScheduleBBC2));
            BuildTeletextPage(_ts.CreateSchedule(CeefaxSectionType.TVScheduleBBC4));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Entertainment));

            // Close
            BuildTeletextPage(Graphics.PromoLinks);

            // Insert stats
            DisplayHtml.AppendLine("<!-- The service started on: {PFC_SERVICESTART} and has built a total of {PFC_TOTALCAROUSELS} carousel(s). -->");
            DisplayHtml.AppendLine("<!-- The service has served {PFC_TOTALREQUESTS} request(s) since starting. -->");
            DisplayHtml.AppendLine("<!-- The latest carousel is: {PFC_TIMESTAMP} taking {PFC_BUILDTIME}ms to build. -->");
            
            // The number of total pages is required javascript page cycler
            DisplayHtml.AppendLine($"<div id='totalPages' style='display:none'>{MaxPages}</div>");
        }
        catch(Exception ex) 
        {
            Log.Fatal($"CAROUSEL BUILD ERROR {ex.Message} {ex.Source} {ex.StackTrace}");
        }
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

