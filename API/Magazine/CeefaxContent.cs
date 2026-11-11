using System.Net;
using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Xml;
using API.Architecture;

namespace API.Magazine;

public class CeefaxContent
{
    public List<NewsStory> StoryList { get; set; } = [];
    public List<CachedUri> UriCache { get; set; } = [];
    public List<CeefaxSection> Sections { get; set; } = [];

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
        Sections.Add(new CeefaxSection(CeefaxSectionType.Kent, new Uri("https://feeds.bbci.co.uk/news/england/kent/rss.xml")));
        
        Sections.Add(new CeefaxSection(CeefaxSectionType.Football, new Uri("https://feeds.bbci.co.uk/sport/football/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Rugby, new Uri("https://feeds.bbci.co.uk/sport/rugby-union/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Cricket, new Uri("https://feeds.bbci.co.uk/sport/cricket/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Tennis, new Uri("https://feeds.bbci.co.uk/sport/tennis/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Golf, new Uri("https://feeds.bbci.co.uk/sport/golf/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Motorsport, new Uri("https://feeds.bbci.co.uk/sport/formula1/rss.xml")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.F1Standings, new Uri("https://www.bbc.co.uk/sport/formula1/standings")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.FootballStandings, new Uri("https://www.bbc.co.uk/sport/football/tables")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.Entertainment, new Uri("https://feeds.bbci.co.uk/news/entertainment_and_arts/rss.xml")));
        
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherUK, new Uri("https://www.bbc.co.uk/weather"))); 
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherHeathfield, new Uri("https://www.bbc.co.uk/weather/2647218"))); 
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherSpotHeathfield, new Uri("https://weather-broker-cdn.api.bbci.co.uk/en/observation/rss/2647218")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherSpotCaerphilly, new Uri("https://weather-broker-cdn.api.bbci.co.uk/en/observation/rss/2654089"))); 
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherSpotLivingston, new Uri("https://weather-broker-cdn.api.bbci.co.uk/en/observation/rss/2644204")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherSpotNewquay, new Uri("https://weather-broker-cdn.api.bbci.co.uk/en/observation/rss/2641589")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherSpotInverness, new Uri("https://weather-broker-cdn.api.bbci.co.uk/en/observation/rss/2646088")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherSpotMiddleton, new Uri("https://weather-broker-cdn.api.bbci.co.uk/en/observation/rss/2642586")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherSpotBelfast, new Uri("https://weather-broker-cdn.api.bbci.co.uk/en/observation/rss/2655984")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherSpotCambridge, new Uri("https://weather-broker-cdn.api.bbci.co.uk/en/observation/rss/2653941")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.WeatherSpotWolverhampton, new Uri("https://weather-broker-cdn.api.bbci.co.uk/en/observation/rss/2633691")));
        
        Sections.Add(new CeefaxSection(CeefaxSectionType.TVScheduleBBC1, new Uri("https://www.bbc.co.uk/schedules/p00fzl6x")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.TVScheduleBBC2, new Uri("https://www.bbc.co.uk/schedules/p015pksy")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.TVScheduleBBC4, new Uri("https://www.bbc.co.uk/schedules/p01kv81d")));
                      
        // Add The Register RSS feed
        Sections.Add(new CeefaxSection(CeefaxSectionType.Register, new Uri("https://www.theregister.com/?lab_viewport=rss")));
        
        // Add the Hargreaves Lansdown API markets and currency request to the URL cache
        Sections.Add(new CeefaxSection(CeefaxSectionType.Markets, new Uri("https://www.hl.co.uk/shares")));
        Sections.Add(new CeefaxSection(CeefaxSectionType.MarketsCurrencies, new Uri("https://www.hl.co.uk/ajax/home/currency-json")));
        
        // Add the FT 10-year UK bond yields
        Sections.Add(new CeefaxSection(CeefaxSectionType.MarketsBonds, new Uri("https://markets.ft.com/data/bonds/tearsheet/summary?s=UK10YG")));

        // Add Travel departures
        Sections.Add(new CeefaxSection(CeefaxSectionType.TravelTrains, new Uri("https://www.thetrainline.com/live/departures/stonegate")));
        //Sections.Add(new CeefaxSection(CeefaxSectionType.TravelFlights, new Uri("https://www.gatwickairport.com/flights")));
        
        // Add each section's feed URL to URL cache
        Sections.ForEach(z => UriCache.Add(new CachedUri(z.Feed)));

        // Process the UR cache (first time)
        ProcessUriCache().Wait();

        // Process feeds to determine which full text stories to display
        Sections.FindAll(z => z.TotalPages > 0).ForEach(z => ProcessRSSFeed(z));
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

        foreach (SyndicationItem item in feed.Items)
        {
            // Only add if not a comment piece, or a 'live' scrolling story, or a Register sponsored article
            if (!item.Title.Text.Contains(':') && 
                !item.Title.Text.Contains('?') &&
                !item.Links[0].Uri.ToString().Contains("/live/") &&
                !item.Summary.Text.Contains("PARTNER CONTENT:") &&
                !item.Summary.Text.Contains("SPONSORED FEATURE:"))
            {
                StoryList.Add(new NewsStory(
                    section.Name, 
                    item.Title.Text.Trim(),
                    item.Summary.Text.Trim(),
                    item.PublishDate.UtcDateTime,
                    item.Links[0].Uri));
            }
        }        
    }

/*
    private void ProcessPodcastList()
    {
        string json = UriCache.First(l => l.Location == Sections.First(z => z.Name == CeefaxSectionType.Podcasts).Feed).ContentString;
        RetroTVChannelData data = JsonSerializer.Deserialize<RetroTVChannelData>(json);

        foreach (RetroTVEpisodeData episode in data.EpisodeList)
        {
            StoryList.Add(new NewsStory(
                CeefaxSectionType.Podcasts,
                episode.ShortName,
                episode.Description,
                Convert.ToDateTime(episode.PublishDate),
                new Uri("http://localhost/" + episode.UrlLocal)));
        }
    }
*/
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
