using System.ServiceModel.Syndication;
using System.Text;

namespace PagesFromCeefax
{
    public class MagazineContent
    {
        public readonly List<NewsStory> StoryList = new List<NewsStory>();
        public readonly List<CachedUrl> UrlCache = new List<CachedUrl>();
        public List<MagazineSection> Sections { get; private set; } = new List<MagazineSection>();
        public StringBuilder DisplayHtml = new StringBuilder();
        public int MaxPages = 0;

        public MagazineContent()
        {
            // Initialise magazine sections
            Sections.Add(new MagazineSection(MagazineSectionType.Home, 3, new Uri("http://feeds.bbci.co.uk/news/uk/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.World, 3, new Uri("http://feeds.bbci.co.uk/news/world/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Politics, 2, new Uri("http://feeds.bbci.co.uk/news/politics/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Science, 1, new Uri("http://feeds.bbci.co.uk/news/science_and_environment/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Technology, 1, new Uri("http://feeds.bbci.co.uk/news/technology/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Business, 3, new Uri("http://feeds.bbci.co.uk/news/business/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Sussex, 2, new Uri("http://feeds.bbci.co.uk/news/england/sussex/rss.xml")));

            Sections.Add(new MagazineSection(MagazineSectionType.Football, 2, new Uri("http://feeds.bbci.co.uk/sport/football/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Rugby, 2, new Uri("http://feeds.bbci.co.uk/sport/rugby-union/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Cricket, 2, new Uri("http://feeds.bbci.co.uk/sport/cricket/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Tennis, 2, new Uri("http://feeds.bbci.co.uk/sport/tennis/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Golf, 1, new Uri("http://feeds.bbci.co.uk/sport/golf/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Formula1, 2, new Uri("http://feeds.bbci.co.uk/sport/formula1/rss.xml")));

            Sections.Add(new MagazineSection(MagazineSectionType.Entertainment, 3, new Uri("http://feeds.bbci.co.uk/news/entertainment_and_arts/rss.xml")));
            Sections.Add(new MagazineSection(MagazineSectionType.Weather, 0, new Uri("https://www.bbc.co.uk/weather")));

            // Add each section's feed URL to URL cache
            Sections.ForEach(z => UrlCache.Add(new CachedUrl() { Location = z.Feed }));

            // Process the URL cache (first time)
            ProcessUrlCache().Wait();

            // Process feeds to determine which full text stories to display
            Sections.FindAll(z => z.TotalStories > 0).ForEach(z => ProcessRSSFeed(z));

            // Process the URL cache (second time, now all story URL's are in)
            ProcessUrlCache().Wait();

            // Parse all stories
            StoryList.ForEach(z => z.AddBody(UrlCache.Find(l => l.Location == z.Link)!.Content!));
        }

        private async Task ProcessUrlCache()
        {
            var client = new HttpClient();
            var results = new List<CachedUrl>();

            var requests = UrlCache
                .FindAll(l => l.Content == null)
                .Select(z => FetchPageAsync(z.Location!))
                .ToList();

            await Task.WhenAll(requests);

            var responses = requests.Select(task => task.Result);

            foreach (var r in responses)
            {
                // Extract the message body and update the Url cache
                var item = UrlCache.Find(l => l.Location == r.location);
                if (item is not null)
                {
                    item.Content = await r.httpResponse!.Content.ReadAsStringAsync();
                }
            }
        }

        private void ProcessRSSFeed(MagazineSection section)
        {
            SyndicationFeed feed = Utility.ReadRSSFeed(UrlCache.Find(l => l.Location == section.Feed)!.Content!);

            int storyCount = 0;
            foreach (SyndicationItem item in feed.Items)
            {
                // Only add the story if not already present, and is displayable
                if (!StoryList.Exists(z => z.Link == item.Links[0].Uri) && (storyCount < section.TotalStories))
                {
                    StoryList.Add(new NewsStory(section.Name, item.Title.Text + ".", item.Links[0].Uri));

                    // Add story link to the URL cache to be retrieved later
                    UrlCache.Add(new CachedUrl() { Location = item.Links[0].Uri });

                    storyCount++;
                }
            }
        }

        private struct RetrievalList
        {
            public Uri? location { get; set; }
            public HttpResponseMessage? httpResponse { get; set; }
        }

        private async Task<RetrievalList> FetchPageAsync(Uri location)
        {
            var client = new HttpClient();
            var content = await client.GetAsync(location);
            return new RetrievalList() { location = location, httpResponse = content };
        }

        
    }
}