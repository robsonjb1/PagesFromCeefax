using System.Diagnostics;
using System.Text;
using API.Architecture;
using HtmlAgilityPack;

namespace API.Magazine;

public interface IKindleContent
{
    public List<SpectatorArticle> SpectatorArticles { get; set; }
    public List<SpectatorCartoon> SpectatorCartoons { get; set; }
    public string SpectatorLogoBase64 {get; set;}
}

public class KindleContent : IKindleContent
{
    public List<SpectatorArticle> SpectatorArticles { get; set; } = new();
    public List<SpectatorCartoon> SpectatorCartoons { get; set; } = new();
    public string SpectatorLogoBase64 { get; set;} = String.Empty;
    private List<CachedUrl> UrlCache { get; set; } = new();
    
    public KindleContent()
    {
        // Visit each article and retrieve the full details, including article and author images
        GetArticleList(@"https://www.spectator.co.uk/").Wait();
        
        // Visit each cartoon and retrieve the full details
        GetCartoonList(@"https://www.spectator.co.uk/illustrations/").Wait();
        
        // Process the URL cache
        ProcessUrlCache().Wait();

        // Parse story content
        Parallel.ForEach(SpectatorArticles, a => ReadArticle(a));
        Parallel.ForEach(SpectatorCartoons, c => ReadCartoon(c));

        // Get Spectator logo image
        SpectatorLogoBase64 = Task.Run(async () => await ImageUrlToBase64(new Uri("https://logos-download.com/wp-content/uploads/2016/10/The_Spectator_logo_text_wordmark.png"))).Result;
    }

    private async Task GetArticleList(string specRoot)
    {
        string rootHtml = await FetchPageAsync(new Uri(specRoot)).Result.httpResponse.Content.ReadAsStringAsync();
        HtmlDocument doc = new();
        doc.LoadHtml(rootHtml);

        // Parse index page and construct article list
        var tags = doc.DocumentNode.SelectNodes("//a/@href");
        if(tags != null)
        {
            foreach (var tag in tags)
            {
                string href = tag.Attributes["href"].Value;
                if(href.Contains("/article/"))
                {
                    Uri u = new Uri(href);
                    if(!SpectatorArticles.Exists(z=> z.ArticleUri == u))
                    {
                        SpectatorArticles.Add(new SpectatorArticle(u));
                        UrlCache.Add(new CachedUrl(u));
                    }
                }
            }
        }
    }

    private async void ReadArticle(SpectatorArticle a)
    {
        try
        {
            string articleHtml = UrlCache.Find(z => z.Location == a.ArticleUri).Content;
            HtmlDocument doc = new();
            doc.LoadHtml(articleHtml);

            a.Headline = doc.DocumentNode.SelectNodes("//meta[@property='og:title']")[0].GetAttributeValue("content", String.Empty);
            a.ImageUri = new Uri(doc.DocumentNode.SelectNodes("//meta[@property='og:image']")[0].GetAttributeValue("content", String.Empty));
            a.ImageBase64 = await ImageUrlToBase64(a.ImageUri);

            string author = doc.DocumentNode.SelectNodes("//meta[@name='author']")[0].GetAttributeValue("content", String.Empty);
            DateTime publishDate = Convert.ToDateTime(doc.DocumentNode.SelectNodes("//meta[@property='article:published_time']")[0].GetAttributeValue("content", String.Empty));
            a.PublishDate = $"{publishDate.DayOfWeek} {publishDate.Day} {publishDate.Date.ToString("MMMM")} {publishDate.Year}";
            a.PublishTime = $"{publishDate.Hour:00}:{publishDate.Minute:00}";
            a.Author = $"{author}";
    
            var avatarNode = doc.DocumentNode.SelectNodes("//a[@class='writers-link entry-header__author']/img");
            if(avatarNode != null)
            {
                a.AvatarUri = new Uri(avatarNode[0].GetAttributeValue("src", String.Empty));
                a.AvatarBase64 = await ImageUrlToBase64(a.AvatarUri);
            }
    
            var body = doc.DocumentNode.SelectNodes("//div[@class='entry-content']//p");
            StringBuilder lines = new StringBuilder();
            
            foreach (var p in body)
            {
                if(p.ParentNode.Name == "blockquote")
                {
                    lines.AppendLine("<p><b><i><center>" + p.OuterHtml.Replace("<p>", "").Replace("</p>", "") + "</center></i></b></p>");
                }
                else
                {
                    if(lines.Length == 0)
                    {
                        // Display the dateline in the first paragraph
                        lines.AppendLine(p.OuterHtml.Replace("<p>", $"<p><b>{a.PublishDate[..a.PublishDate.IndexOf(" ")]}, {a.PublishTime}. </b>"));
                    }
                    else
                    {
                        lines.AppendLine(p.OuterHtml);
                    }
                }
            }
            a.StoryHtml = lines.ToString();      
        }
        catch
        {
            a.IsValid = false;
        }
    }

    private async void ReadCartoon(SpectatorCartoon c)
    {
        try
        {
            string cartoonHtml = UrlCache.Find(z => z.Location == c.CartoonUri).Content;
            HtmlDocument doc = new();
            doc.LoadHtml(cartoonHtml);

            c.Caption = doc.DocumentNode.SelectNodes("//meta[@property='og:description']")[0].GetAttributeValue("content", String.Empty);
            c.Caption = c.Caption.StartsWith("Weekly magazine") ? String.Empty : c.Caption;
            c.ImageUri = new Uri(doc.DocumentNode.SelectNodes("//meta[@property='og:image']")[0].GetAttributeValue("content", String.Empty));
            c.ImageBase64 = await ImageUrlToBase64(c.ImageUri);
        }
        catch
        {
            c.IsValid = false;
        }
    }

    private async Task GetCartoonList(string specRoot)
    {
        string rootHtml = await FetchPageAsync(new Uri(specRoot)).Result.httpResponse.Content.ReadAsStringAsync();
        HtmlDocument doc = new();
        doc.LoadHtml(rootHtml);

        // Parse index page and construct article list
        var tags = doc.DocumentNode.SelectNodes("//a[@class='article__title-link']");
        if(tags != null)
        {
            foreach (var tag in tags)
            {
                Uri u = new Uri(tag.Attributes["href"].Value);
                SpectatorCartoons.Add(new SpectatorCartoon(u));
                UrlCache.Add(new CachedUrl(u));
            }
        }
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

    private static async Task<(Uri location, HttpResponseMessage httpResponse)> FetchPageAsync(Uri location)
    {
        var client = new HttpClient();
        var content = await client.GetAsync(location);
        return (location, content);
    }

    public async Task<string> ImageUrlToBase64(Uri imageUri)
    {
        using var httpClient = new HttpClient();
        var imageBytes = await httpClient.GetByteArrayAsync(imageUri);                
        return Convert.ToBase64String(imageBytes);
    }
}