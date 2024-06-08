using System.Diagnostics;
using System.Text;
using API.Architecture;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public interface IKindleContent
{
    public List<SpectatorArticle> SpectatorArticles { get; set; }
    public List<SpectatorCartoon> SpectatorCartoons { get; set; }
    public string SpectatorLogoBase64 { get; set; }
    public List<RegisterArticle> RegisterArticles { get; set; }
    public string RegisterLogoBase64 { get; set; }
}

public class KindleContent : IKindleContent
{
    public List<SpectatorArticle> SpectatorArticles { get; set; } = new();
    public List<SpectatorCartoon> SpectatorCartoons { get; set; } = new();
    public string SpectatorLogoBase64 { get; set; } = String.Empty;
    public List<RegisterArticle> RegisterArticles { get; set; } = new();
    public string RegisterLogoBase64 { get; set; } = String.Empty;
    private List<CachedUrl> UrlCache { get; set; } = new();
    private ISystemConfig _config;
 
    public KindleContent(ISystemConfig config)
    {
        _config = config;

        // Visit each article and retrieve the full details, including article and author images
        GetSpectatorArticleList(@"https://www.spectator.co.uk/").Wait();
        //GetSpectatorCartoonList(@"https://www.spectator.co.uk/illustrations/").Wait();
        //GetRegisterArticleList(@"https://www.theregister.com/").Wait();

        // Process the URL cache
        //ProcessUrlCache().Wait();
        UrlCache.ForEach(u => u.Content = FetchPageAsync(u.Location).Result.httpResponse.Content.ReadAsStringAsync().Result);
        
        // Parse story content
        SpectatorArticles.ForEach(a => ReadSpectatorArticle(a));
        //Parallel.ForEach(SpectatorCartoons, c => ReadSpectatorCartoon(c));
        //Parallel.ForEach(RegisterArticles, r => ReadRegisterArticle(r));

        // Get Spectator logo image
        //SpectatorLogoBase64 = Task.Run(async () => await ImageUrlToBase64(new Uri("https://logos-download.com/wp-content/uploads/2016/10/The_Spectator_logo_text_wordmark.png"))).Result;

        // Get The Register logo image
        //RegisterLogoBase64 = Task.Run(async () => await ImageUrlToBase64(new Uri("https://www.theregister.com/design_picker/1fea2ae01c5036112a295123c3cc9c56eb28836a/graphics/std/red_logo_sans_strapline.png"))).Result;
    }

    private async Task GetSpectatorArticleList(string root)
    {
        string rootHtml = await FetchPageAsync(new Uri(root)).Result.httpResponse.Content.ReadAsStringAsync();
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

    private void ReadSpectatorArticle(SpectatorArticle a)
    {
        try
        {
            string articleHtml = UrlCache.Find(z => z.Location == a.ArticleUri).Content;
            HtmlDocument doc = new();
            doc.LoadHtml(articleHtml);

            a.Headline = doc.DocumentNode.SelectNodes("//meta[@property='og:title']")[0].GetAttributeValue("content", String.Empty);
            a.ImageUri = new Uri(doc.DocumentNode.SelectNodes("//meta[@property='og:image']")[0].GetAttributeValue("content", String.Empty));
            a.ImageBase64 = ImageUrlToBase64(a.ImageUri).Result;

            string author = doc.DocumentNode.SelectNodes("//meta[@name='author']")[0].GetAttributeValue("content", String.Empty);
            DateTime publishDate = Convert.ToDateTime(doc.DocumentNode.SelectNodes("//meta[@property='article:published_time']")[0].GetAttributeValue("content", String.Empty));
            a.PublishDate = $"{publishDate.DayOfWeek} {publishDate.Day} {publishDate.Date.ToString("MMMM")} {publishDate.Year}";
            a.PublishTime = $"{publishDate.Hour:00}:{publishDate.Minute:00}";
            a.Author = $"{author}";

            var avatarNode = doc.DocumentNode.SelectNodes("//a[@class='writers-link entry-header__author']/img");
            if(avatarNode != null)
            {
                a.AvatarUri = new Uri(avatarNode[0].GetAttributeValue("src", String.Empty));
                a.AvatarBase64 = ImageUrlToBase64(a.AvatarUri).Result;
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
                        lines.AppendLine(p.OuterHtml.Replace("<p>", $"<p><b>{a.PublishDate[..a.PublishDate.IndexOf(" ")]}. </b>"));
                    }
                    else
                    {
                        lines.AppendLine(p.OuterHtml);
                    }
                }
            }
        
            a.StoryHtml = lines.ToString(); 
            a.IsValid = true;     
        }
        catch
        {
            Log.Error($"Exception occured processing article {a.ArticleUri}");
        }
    }

    private async void ReadSpectatorCartoon(SpectatorCartoon c)
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

    private async Task GetSpectatorCartoonList(string root)
    {
        string rootHtml = await FetchPageAsync(new Uri(root)).Result.httpResponse.Content.ReadAsStringAsync();
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
   
    private async Task GetRegisterArticleList(string root)
    {
        string rootHtml = await FetchPageAsync(new Uri(root)).Result.httpResponse.Content.ReadAsStringAsync();
        HtmlDocument doc = new();
        doc.LoadHtml(rootHtml);

        // Parse index page and construct article list
        var tags = doc.DocumentNode.SelectNodes("//article");
        if(tags != null)
        {
            foreach (var tag in tags)
            {               
                string href = tag.SelectSingleNode(".//a").Attributes["href"].Value;
                if(href.StartsWith("/")) 
                {
                    href = "https://www.theregister.com" + href;
                }
                Uri u = new Uri(href);
                if (!RegisterArticles.Exists(z=> z.ArticleUri == u))
                {
                    RegisterArticles.Add(new RegisterArticle(u));
                    UrlCache.Add(new CachedUrl(u));
                }
            }
        }
    }

    private void ReadRegisterArticle(RegisterArticle a)
    {
        try
        {
            string articleHtml = UrlCache.Find(z => z.Location == a.ArticleUri).Content;
            HtmlDocument doc = new();
            doc.LoadHtml(articleHtml);

            var articleRoot = doc.DocumentNode.SelectSingleNode("//article");
            a.Headline = articleRoot.SelectSingleNode(".//h1").InnerText.Trim();
            a.Byline = articleRoot.SelectSingleNode(".//h2").InnerText.Trim();
            a.Section = articleRoot.SelectSingleNode(".//h4").InnerText.Trim().ToUpper();
            a.PublishDate = Convert.ToDateTime(articleHtml.Substring(articleHtml.IndexOf("\"datePublished\":\"") + 17, 19));
            
            var body = articleRoot.SelectNodes(".//p");
            StringBuilder lines = new StringBuilder();
            foreach (var p in body)
            {
                if(p.ParentNode.Name == "blockquote")
                {
                    lines.AppendLine("<b><i><center>" + p.OuterHtml + "</center></i></b>");
                }
                else
                {
                    if(p.ParentNode.GetAttributeValue("class", "") != "tip_off_widget")
                    {
                        lines.AppendLine(p.OuterHtml);
                    }
                }
            }
            a.StoryHtml = $"<p><b>{a.PublishDate.DayOfWeek}, {a.PublishDate.Hour.ToString().PadLeft(2, '0')}:{a.PublishDate.Minute.ToString().PadLeft(2, '0')}</b>. {lines.ToString().Substring(3)}"; 
        }
        catch
        {
            a.IsValid = false;
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
                if(httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Received status code {httpResponse.StatusCode} for {location}");
                }

                // Extract the message body and update the Url cache
                var item = UrlCache.Find(l => l.Location == location);
                if (item is not null)
                {
                    item.Content = await httpResponse.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine($"Could not find {location} in UrlCache");
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"URL fetch error: {ex.Message} {ex.Source}");
        }
    }

    private async Task<(Uri location, HttpResponseMessage httpResponse)> FetchPageAsync(Uri location)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Cookie", _config.SpecSessionCookie);
       
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