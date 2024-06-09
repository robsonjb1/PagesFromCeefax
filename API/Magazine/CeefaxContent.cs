using System.ServiceModel.Syndication;
using System.Xml;
using API.Architecture;

namespace API.Magazine;

public interface ICeefaxContent
{
    public List<NewsStory> StoryList { get; set; }
    public List<CachedUri> UrlCache { get; set; }
    public List<CeefaxSection> Sections { get; set; }
}

public class CeefaxContent : ICeefaxContent
{
    public List<NewsStory> StoryList { get; set; } = new();
    public List<CachedUri> UrlCache { get; set; } = new();
    public List<CeefaxSection> Sections { get; set; } = new();

    private readonly ISystemConfig _config;

    public CeefaxContent(ISystemConfig config)
    {
        _config = config;

        // Initialise magazine sections
        Sections.Add(new CeefaxSection(CeefaxSectionType.Home, new Uri("http://feeds.bbci.co.uk/news/uk/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.World, new Uri("http://feeds.bbci.co.uk/news/world/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Politics, new Uri("http://feeds.bbci.co.uk/news/politics/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Science, new Uri("http://feeds.bbci.co.uk/news/science_and_environment/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Technology, new Uri("http://feeds.bbci.co.uk/news/technology/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Business, new Uri("http://feeds.bbci.co.uk/news/business/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Sussex, new Uri("http://feeds.bbci.co.uk/news/england/sussex/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Football, new Uri("http://feeds.bbci.co.uk/sport/football/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Rugby, new Uri("http://feeds.bbci.co.uk/sport/rugby-union/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Cricket, new Uri("http://feeds.bbci.co.uk/sport/cricket/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Tennis, new Uri("http://feeds.bbci.co.uk/sport/tennis/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Golf, new Uri("http://feeds.bbci.co.uk/sport/golf/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Formula1, new Uri("http://feeds.bbci.co.uk/sport/formula1/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Entertainment, new Uri("http://feeds.bbci.co.uk/news/entertainment_and_arts/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherForecast, new Uri("https://www.bbc.co.uk/weather")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Markets, new Uri("https://www.bbc.co.uk/news/business/market-data")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.TVScheduleBBC1, new Uri("https://www.bbc.co.uk/schedules/p00fzl6x")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.TVScheduleBBC2, new Uri("https://www.bbc.co.uk/schedules/p015pksy")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.TVScheduleBBC4, new Uri("https://www.bbc.co.uk/schedules/p01kv81d")));
                  
        AddWeatherTempSection(CeefaxSectionType.WeatherTempLondon, "London");
        AddWeatherTempSection(CeefaxSectionType.WeatherTempBelfast, "Belfast");
        AddWeatherTempSection(CeefaxSectionType.WeatherTempCardiff, "Cardiff");
        AddWeatherTempSection(CeefaxSectionType.WeatherTempEdinburgh, "Edinburgh");
        AddWeatherTempSection(CeefaxSectionType.WeatherTempLerwick, "Lerwick");
        AddWeatherTempSection(CeefaxSectionType.WeatherTempManchester, "Manchester");
        AddWeatherTempSection(CeefaxSectionType.WeatherTempTruro, "Truro");

        // Add each section's feed URL to URL cache
        Sections.ForEach(z => UrlCache.Add(new CachedUri(z.Feed)));

        // Process the URL cache (first time)
        ProcessUrlCache().Wait();

        // Process feeds to determine which full text stories to display
        Sections.FindAll(z => z.TotalStories > 0).ForEach(z => ProcessRSSFeed(z));
        
        // Process the URL cache (second time, now all story URL's are in)
        ProcessUrlCache().Wait();

        // Parse all stories
        StoryList.ForEach(z => z.AddBody(UrlCache.Find(l => l.Location == z.Link).ContentString));
    }

    private void AddWeatherTempSection(CeefaxSectionType section, string city)
    {
        Sections.Add(new CeefaxSection(section, new Uri($"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={_config.OpenWeatherApiKey}")));
    }

    private async Task ProcessUrlCache()
    {
        try
        {
            var client = new HttpClient();
            var results = new List<CachedUri>();

            var requests = UrlCache
                .FindAll(l => l.ContentString == null)
                .Select(z => FetchPageAsync(z.Location!))
                .ToList();

            await Task.WhenAll(requests);

            var responses = requests.Select(task => task.Result);

            foreach (var (location, httpResponse) in responses)
            {
                // Extract the message body and update the Url cache
                var item = UrlCache.Find(l => l.Location == location);
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
        TextReader tr = new StringReader(UrlCache.Find(l => l.Location == section.Feed).ContentString);
        SyndicationFeed feed = SyndicationFeed.Load(XmlReader.Create(tr));

        int storyCount = 0;
        foreach (SyndicationItem item in feed.Items)
        {
            // Only add the story if not already present, and is not marked as a 'live' scrolling story
            if (!StoryList.Exists(z => z.Link == item.Links[0].Uri) && (storyCount < section.TotalStories) && !item.Links[0].Uri.ToString().Contains("/live/"))
            {
                StoryList.Add(new NewsStory(section.Name, item.Title.Text.Trim() + ".", item.Links[0].Uri));

                // Add story link to the URL cache to be retrieved later
                UrlCache.Add(new CachedUri(item.Links[0].Uri));
            
                storyCount++;
            }
        }
    }

    private static async Task<(Uri location, HttpResponseMessage httpResponse)> FetchPageAsync(Uri location)
    {
        var client = new HttpClient();
        var content = await client.GetAsync(location);
        return (location, content);
    }
}