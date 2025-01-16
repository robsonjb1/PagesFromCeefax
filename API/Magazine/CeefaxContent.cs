using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using API.Architecture;

namespace API.Magazine;

public interface ICeefaxContent
{
    public List<NewsStory> StoryList { get; set; }
    public List<CachedUri> UriCache { get; set; }
    public List<CeefaxSection> Sections { get; set; }
}

public class CeefaxContent : ICeefaxContent
{
    public List<NewsStory> StoryList { get; set; } = new();
    public List<CachedUri> UriCache { get; set; } = new();
    public List<CeefaxSection> Sections { get; set; } = new();

    private readonly ISystemConfig _config;

    public CeefaxContent(ISystemConfig config)
    {
        _config = config;

        // Initialise magazine sections
        Sections.Add(new CeefaxSection(CeefaxSectionType.Home, new Uri("https://feeds.bbci.co.uk/news/uk/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.World, new Uri("https://feeds.bbci.co.uk/news/world/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Politics, new Uri("https://feeds.bbci.co.uk/news/politics/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Science, new Uri("https://feeds.bbci.co.uk/news/science_and_environment/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Technology, new Uri("https://feeds.bbci.co.uk/news/technology/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Business, new Uri("https://feeds.bbci.co.uk/news/business/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Sussex, new Uri("https://feeds.bbci.co.uk/news/england/sussex/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Football, new Uri("https://feeds.bbci.co.uk/sport/football/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Rugby, new Uri("https://feeds.bbci.co.uk/sport/rugby-union/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Cricket, new Uri("https://feeds.bbci.co.uk/sport/cricket/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Tennis, new Uri("https://feeds.bbci.co.uk/sport/tennis/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Golf, new Uri("https://feeds.bbci.co.uk/sport/golf/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Formula1, new Uri("https://feeds.bbci.co.uk/sport/formula1/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Standings, new Uri("https://www.bbc.co.uk/sport/formula1/standings")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Entertainment, new Uri("https://feeds.bbci.co.uk/news/entertainment_and_arts/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Weather, new Uri("https://www.bbc.co.uk/weather")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Markets, new Uri("https://www.hl.co.uk/shares")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.TVScheduleBBC1, new Uri("https://www.bbc.co.uk/schedules/p00fzl6x")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.TVScheduleBBC2, new Uri("https://www.bbc.co.uk/schedules/p015pksy")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.TVScheduleBBC4, new Uri("https://www.bbc.co.uk/schedules/p01kv81d")));
        
        // Add each section's feed URL to URL cache
        Sections.ForEach(z => UriCache.Add(new CachedUri(z.Feed)));

        // Add the weather API requests to the URL cache
        SystemConfig.WeatherCities.ForEach(c => AddWeatherAPIUriToCache(c));
        
        // Add the Hargreaves Lansdown API currency request to the URL cache
        UriCache.Add(new CachedUri(new Uri("https://www.hl.co.uk/ajax/home/currency-json"), "HL-Currencies"));

        // Add Yahoo requests (market close) to the URL cache
        UriCache.Add(new CachedUri(new Uri("https://finance.yahoo.com/quote/%5EFTSE/"), "YH-UKX"));
        UriCache.Add(new CachedUri(new Uri("https://finance.yahoo.com/quote/%5EFTMC/"), "YH-MCX"));
        UriCache.Add(new CachedUri(new Uri("https://finance.yahoo.com/quote/%5EFCHI/"), "YH-CAC"));
        UriCache.Add(new CachedUri(new Uri("https://finance.yahoo.com/quote/%5EGDAXI/"), "YH-DAX"));
        UriCache.Add(new CachedUri(new Uri("https://finance.yahoo.com/quote/%5EDJI/"), "YH-DJIA"));
        UriCache.Add(new CachedUri(new Uri("https://finance.yahoo.com/quote/%5EIXIC/"), "YH-COMP"));
        UriCache.Add(new CachedUri(new Uri("https://finance.yahoo.com/quote/%5EHSI/"), "YH-HSI"));
        UriCache.Add(new CachedUri(new Uri("https://finance.yahoo.com/quote/%5EN225/"), "YH-NK225"));

        // 10-year UK bond yields
        UriCache.Add(new CachedUri(new Uri("https://uk.investing.com/rates-bonds/uk-10-year-bond-yield-historical-data"), "UK-10YRBOND"));

        // Process the UR cache (first time)
        ProcessUriCache().Wait();

        // Process feeds to determine which full text stories to display
        Sections.FindAll(z => z.TotalStories > 0).ForEach(z => ProcessRSSFeed(z));
        
        // Process the URI cache (second time, now all story URL's are in)
        ProcessUriCache().Wait();

        // Parse all stories
        StoryList.ForEach(z => z.AddBody(UriCache.Find(l => l.Location == z.Link).ContentString));
    }

    private void AddWeatherAPIUriToCache(string city)
    {
        UriCache.Add(new CachedUri(
            new Uri($"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={_config.OpenWeatherApiKey}"),
            city));
    }

    private async Task ProcessUriCache()
    {
        try
        {
            var client = new HttpClient();
            var results = new List<CachedUri>();

            var requests = UriCache
                .FindAll(l => l.ContentString == null)
                .Select(z => FetchPageAsync(z.Location!))
                .ToList();

            await Task.WhenAll(requests);

            var responses = requests.Select(task => task.Result);

            foreach (var (location, httpResponse) in responses)
            {
                // Extract the message body and update the Url cache
                var item = UriCache.Find(l => l.Location == location);
                if (item is not null)
                {
                    item.ContentString = await httpResponse.Content.ReadAsStringAsync();
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"URL fetch error: {ex.Message} {ex.Source}");
        }
    }

    private void ProcessRSSFeed(CeefaxSection section)
    {
        TextReader tr = new StringReader(UriCache.Find(l => l.Location == section.Feed).ContentString);
        SyndicationFeed feed = SyndicationFeed.Load(XmlReader.Create(tr));

        int storyCount = 0;
        foreach (SyndicationItem item in feed.Items)
        {
            // Only add the story if not already present, and is not marked as a 'live' scrolling story
            if (!StoryList.Exists(z => z.Link == item.Links[0].Uri) && (storyCount < section.TotalStories) && !item.Title.Text.StartsWith("Watch:") && !item.Links[0].Uri.ToString().Contains("/live/"))
            {
                StoryList.Add(new NewsStory(section.Name, item.Title.Text.Trim(), item.Links[0].Uri));

                // Add story link to the URL cache to be retrieved later
                UriCache.Add(new CachedUri(item.Links[0].Uri));
            
                storyCount++;
            }
        }
    }

    private static async Task<(Uri location, HttpResponseMessage httpResponse)> FetchPageAsync(Uri location)
    {
        var httpHandler = new SocketsHttpHandler() {
            AutomaticDecompression = DecompressionMethods.GZip
                                    | DecompressionMethods.Deflate
        };

        var client = new HttpClient(httpHandler);
        var content = await client.GetAsync(location);
        return (location, content);
    }
}
