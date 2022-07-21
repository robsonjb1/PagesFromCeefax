using System.ServiceModel.Syndication;
using System.Xml;
using API.Architecture;
using API.DataTransferObjects;

namespace API.Magazine
{

    public interface IMagazineContent
    {
        public List<NewsStory> StoryList { get; set; }
        public List<CachedUrl> UrlCache { get; set; }
        public List<MagazineSection> Sections { get; set; }

        public void RefreshContent();
    }

    public class MagazineContent : IMagazineContent
    {
        public List<NewsStory> StoryList { get; set; }
        public List<CachedUrl> UrlCache { get; set; }
        public List<MagazineSection> Sections { get; set; }
       
        private readonly ISystemConfig _config;

        public MagazineContent(ISystemConfig config)
        {
            _config = config;
            RefreshContent();
        }

        public void RefreshContent()
        {
            StoryList = new List<NewsStory>();
            UrlCache = new List<CachedUrl>();
            Sections = new List<MagazineSection>();

            // Initialise magazine sections
            Sections.Add(new MagazineSection(MagazineSectionType.Home, new Uri("http://feeds.bbci.co.uk/news/uk/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.World, new Uri("http://feeds.bbci.co.uk/news/world/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Politics, new Uri("http://feeds.bbci.co.uk/news/politics/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Science, new Uri("http://feeds.bbci.co.uk/news/science_and_environment/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Technology, new Uri("http://feeds.bbci.co.uk/news/technology/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Business, new Uri("http://feeds.bbci.co.uk/news/business/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Sussex, new Uri("http://feeds.bbci.co.uk/news/england/sussex/rss.xml")));

            Sections.Add(new MagazineSection(MagazineSectionType.Football, new Uri("http://feeds.bbci.co.uk/sport/football/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Rugby, new Uri("http://feeds.bbci.co.uk/sport/rugby-union/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Cricket, new Uri("http://feeds.bbci.co.uk/sport/cricket/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Tennis, new Uri("http://feeds.bbci.co.uk/sport/tennis/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Golf, new Uri("http://feeds.bbci.co.uk/sport/golf/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Formula1, new Uri("http://feeds.bbci.co.uk/sport/formula1/rss.xml")));

            Sections.Add(new MagazineSection(MagazineSectionType.Entertainment, new Uri("http://feeds.bbci.co.uk/news/entertainment_and_arts/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.WeatherForecast, new Uri("https://www.bbc.co.uk/weather")));
           
            AddWeatherTempSection(MagazineSectionType.WeatherTempLondon, "London");
            AddWeatherTempSection(MagazineSectionType.WeatherTempBelfast, "Belfast");
            AddWeatherTempSection(MagazineSectionType.WeatherTempCardiff, "Cardiff");
            AddWeatherTempSection(MagazineSectionType.WeatherTempEdinburgh, "Edinburgh");
            AddWeatherTempSection(MagazineSectionType.WeatherTempLerwick, "Lerwick");
            AddWeatherTempSection(MagazineSectionType.WeatherTempManchester, "Manchester");

            // Add each section's feed URL to URL cache
            Sections.ForEach(z => UrlCache.Add(new CachedUrl(z.Feed)));

            // Process the URL cache (first time)
            ProcessUrlCache().Wait();

            // Process feeds to determine which full text stories to display
            Sections.FindAll(z => z.TotalStories > 0).ForEach(z => ProcessRSSFeed(z));

            // Process the URL cache (second time, now all story URL's are in)
            ProcessUrlCache().Wait();

            // Parse all stories
            StoryList.ForEach(z => z.AddBody(UrlCache.Find(l => l.Location == z.Link).Content));
        }

        private void AddWeatherTempSection(MagazineSectionType section, string city)
        {
            Sections.Add(new MagazineSection(section, new Uri($"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={_config.OpenWeatherApiKey}")));
        }

        private async Task ProcessUrlCache()
        {
            try
            {
                var client = new HttpClient();
                var results = new List<CachedUrl>();

                var requests = UrlCache
                    .FindAll(l => l.Content == null)
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
                        item.Content = await httpResponse.Content.ReadAsStringAsync();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"URL fetch error: {ex.Message} {ex.Source}");
            }
        }

        private void ProcessRSSFeed(MagazineSection section)
        {
            TextReader tr = new StringReader(UrlCache.Find(l => l.Location == section.Feed).Content);
            SyndicationFeed feed = SyndicationFeed.Load(XmlReader.Create(tr));

            int storyCount = 0;
            foreach (SyndicationItem item in feed.Items)
            {
                // Only add the story if not already present, and is displayable
                if (!StoryList.Exists(z => z.Link == item.Links[0].Uri) && (storyCount < section.TotalStories))
                {
                    StoryList.Add(new NewsStory(section.Name, item.Title.Text + ".", item.Links[0].Uri));

                    // Add story link to the URL cache to be retrieved later
                    UrlCache.Add(new CachedUrl(item.Links[0].Uri));
                
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
}