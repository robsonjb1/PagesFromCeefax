using System.Text;
using API.Architecture;
using API.PageGenerators;
using Serilog;

namespace API.Services;

public class TeletextPage
{
    public bool IsValid { get; set; } = true;
    public int[] Data { get; set; } = new int[1000]; // 40*25 bytes
}

public class TeletextCarousel
{
    public bool IsValid { get; set; }
    public int TotalPages { get; set; } = 0;
    public List<TeletextPage> Content { get; set; } = [];
}

public class CarouselService
{
    public TeletextCarousel Carousel { get; set; } = new();
    private ITeletextPageWeather _tw;
    private ITeletextPageNews _tn;
    private ITeletextPageMarkets _tm;
    private ITeletextPageTV _tt;
    private ITeletextPageStanding _ts;
   
    public CarouselService(ITeletextPageNews tn, ITeletextPageMarkets tm, ITeletextPageStanding ts, ITeletextPageWeather tw, ITeletextPageTV tt)
    {
        _tn = tn;
        _tm = tm;
        _ts = ts;
        _tw = tw;
        _tt = tt;
   
        try
        {
            // This is teletext
            BuildTeletextPage(Graphics.PromoTeletext);

            // News section
            BuildTeletextPage(Graphics.PromoNews);
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Home));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.World));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Politics));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Science));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Technology));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Sussex));

            // Business section
            BuildTeletextPage(Graphics.PromoBusiness);
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Business));
            BuildTeletextPage(_tm.CreateMarketsPage());
           
            // Sports section
            BuildTeletextPage(Graphics.PromoSport);
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Football));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Rugby));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Cricket));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Tennis));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Golf));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Formula1));
            BuildTeletextPage(_ts.CreateStandingsPage());

            // Weather section
            BuildTeletextPage(Graphics.PromoWeather);
            BuildTeletextPage(_tw.CreateWeatherMap());
            BuildTeletextPage(_tw.CreateWeatherPage(CeefaxSectionType.WeatherForecast1));
            BuildTeletextPage(_tw.CreateWeatherPage(CeefaxSectionType.WeatherForecast2));
            BuildTeletextPage(_tw.CreateWeatherPage(CeefaxSectionType.WeatherForecast3));
             
            // Entertainment section
            BuildTeletextPage(Graphics.PromoTV);
            BuildTeletextPage(_tt.CreateSchedule(CeefaxSectionType.TVScheduleBBC1));
            BuildTeletextPage(_tt.CreateSchedule(CeefaxSectionType.TVScheduleBBC2));
            BuildTeletextPage(_tt.CreateSchedule(CeefaxSectionType.TVScheduleBBC4));
            BuildTeletextPage(_tn.CreateNewsSection(CeefaxSectionType.Entertainment));

            // Close
            BuildTeletextPage(Graphics.PromoLinks);
            if(DateTime.Now.Month == 12) {
                BuildTeletextPage(Graphics.PromoChristmas);
            }            
        }
        catch(Exception ex) 
        {
            Log.Fatal($"CAROUSEL BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }

    #region Private Methods
    private void BuildTeletextPage(StringBuilder newPage)
    {
        if(newPage.Length > 0)    // The page will not render if it's data object could not be created
        {
            TeletextPage page = new();
            int row = 0;
            int dataPos = 40;     // Skip the first header row, this will be control by the viewing application
            string[] lines = newPage.ToString().Split(Environment.NewLine);
            foreach (string line in lines.Take(lines.Count() - 1))
            {
                row++;
                if(row == 24) { break; }

                char[] chars = Utility.ConvertToCharArray(line);
                if(chars.Length > 40) { page.IsValid = false; }
                for(int i = 0; i < 40; i++)
                {
                    int ascii = (int)chars[i];
                    if(ascii > 127) // We can't display this character
                    {
                        ascii = 128;
                        page.IsValid = false;
                    }
                    
                    page.Data[dataPos] = ascii;
                    dataPos++;
                }
            }

            // Check we have exactly 23 rows
            if(row != 23) { page.IsValid = false; }

            Carousel.Content.Add(page);
            Carousel.TotalPages++;
        }
    }

    private void BuildTeletextPage(List<StringBuilder> newPages)
    {
        newPages.ForEach(z => BuildTeletextPage(z));
    }
    #endregion
}
