using System.Text;
using API.Architecture;
using API.PageGenerators;
using Serilog;

namespace API.Services;

public interface ICarouselService
{
    public string GetCarousel(bool errorsDetected);
}

public class CarouselService : ICarouselService
{
    private int _totalPages { get; set; } = 0;
    private readonly StringBuilder _displayHtml = new();
    private ITeletextPageWeather _tw;
    private ITeletextPageNews _tn;
    private ITeletextPageMarkets _tm;
    private ITeletextPageTV _tt;
   
    public CarouselService(ITeletextPageNews tn, ITeletextPageWeather tw, ITeletextPageMarkets tm, ITeletextPageTV tt)
    {
        _tw = tw;
        _tn = tn;
        _tm = tm;
        _tt = tt;
    }

    public string GetCarousel(bool isValid)
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

            // Business section
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
            BuildTeletextPage(_tw.CreateWeatherPage(CeefaxSectionType.WeatherForecast1));
            BuildTeletextPage(_tw.CreateWeatherPage(CeefaxSectionType.WeatherForecast2));
            BuildTeletextPage(_tw.CreateWeatherPage(CeefaxSectionType.WeatherForecast3));
            BuildTeletextPage(_tw.CreateWeatherWorld());
            
            // Entertainment section
            BuildTeletextPage(Graphics.PromoTV);
            BuildTeletextPage(_tt.CreateSchedule(CeefaxSectionType.TVScheduleBBC1));
            BuildTeletextPage(_tt.CreateSchedule(CeefaxSectionType.TVScheduleBBC2));
            BuildTeletextPage(_tt.CreateSchedule(CeefaxSectionType.TVScheduleBBC4));
            BuildTeletextPage(_tn.CreateNewsPage(CeefaxSectionType.Entertainment));

            // Close
            BuildTeletextPage(Graphics.PromoLinks);
            if(DateTime.Now.Month == 12) {
                BuildTeletextPage(Graphics.PromoChristmas);
            }

            // Insert stats
            _displayHtml.AppendLine("<!-- The service started on: {PFC_SERVICESTART} and has built a total of {PFC_TOTALCAROUSELS} carousel(s). -->");
            _displayHtml.AppendLine("<!-- The service has served {PFC_TOTALREQUESTS} request(s) since starting. -->");
            _displayHtml.AppendLine("<!-- The latest carousel is: {PFC_TIMESTAMP} taking {PFC_BUILDTIME}ms to build. -->");
            
            // The number of total pages is required javascript page cycler
            _displayHtml.AppendLine($"<div id='totalPages' style='display:none'>{_totalPages}</div>");
            _displayHtml.AppendLine($"<div id='isValid' style='display:none'>{isValid}</div>");
        }
        catch(Exception ex) 
        {
            Log.Fatal($"CAROUSEL BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
        return _displayHtml.ToString();
    }

    #region Private Methods
    private void BuildTeletextPage(StringBuilder newPage)
    {
        if(newPage.Length > 0)    // The page will not render if it's data object could not be created
        {
            _totalPages++;

            // Generate the <div> enclosure that contains the individual page
            _displayHtml.AppendLine($"<div id=\"page{_totalPages}\" style=\"display:none\">");
            _displayHtml.Append(newPage);

            _displayHtml.Append("<div class=\"pageBreak\"><br><br><br></div>");
            _displayHtml.Append("</div>");
        }
    }

    private void BuildTeletextPage(List<StringBuilder> newPages)
    {
        newPages.ForEach(z => BuildTeletextPage(z));
    }
    #endregion
}
