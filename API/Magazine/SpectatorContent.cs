using System.Text;
using System.Text.RegularExpressions;
using API.Architecture;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public interface ISpectatorContent
{
    public List<SpectatorArticle> SpectatorArticles { get; set; }
}

public class SpectatorContent : ISpectatorContent
{
    public List<SpectatorArticle> SpectatorArticles { get; set; } = new();
    private List<CachedUri> UrlCache { get; set; } = new();
    private List<CachedUri> ImageCache { get; set; } = new();
    private ISystemConfig _config;
 
    public SpectatorContent(ISystemConfig config)
    {
        _config = config;

        // Visit each article and retrieve the full details, including article and author images
        GetSpectatorArticleList(@"https://www.spectator.co.uk/").Wait();
     
        // Process the URL article cache (deliberately do this sequentially)
        UrlCache.ForEach(u => u.ContentString = FetchPageAsync(u.Location).Result.httpResponse.Content.ReadAsStringAsync().Result);
        
        // Parse story content
        SpectatorArticles.ForEach(a => ReadSpectatorArticle(a));

        // Process the URL image cache
        Parallel.ForEach(ImageCache, i => i.ContentBytes = FetchPageAsync(i.Location).Result.httpResponse.Content.ReadAsByteArrayAsync().Result);
        foreach(var a in SpectatorArticles)
        {
            if(a.ImageUri != null)
            {
                a.ImageBase64 = Convert.ToBase64String(ImageCache.Find(z => z.Location == a.ImageUri).ContentBytes);
            }

            if(a.AvatarUri != null)
            {
                a.AvatarBase64 = Convert.ToBase64String(ImageCache.Find(z => z.Location == a.AvatarUri).ContentBytes);
            }
        }
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
                        UrlCache.Add(new CachedUri(u));
                    }
                }
            }
        }
    }

    private void ReadSpectatorArticle(SpectatorArticle a)
    {
        try
        {
            string articleHtml = UrlCache.Find(z => z.Location == a.ArticleUri).ContentString;
            HtmlDocument doc = new();
            doc.LoadHtml(articleHtml);

            a.Headline = doc.DocumentNode.SelectNodes("//meta[@property='og:title']")[0].GetAttributeValue("content", String.Empty);
            DateTime publishDate = Convert.ToDateTime(doc.DocumentNode.SelectNodes("//meta[@property='article:published_time']")[0].GetAttributeValue("content", String.Empty));
            a.PublishDate = $"{publishDate.DayOfWeek}, {publishDate.Day}{Utility.GetDaySuffix(publishDate)} {publishDate:MMMM}";
            string author = doc.DocumentNode.SelectNodes("//meta[@name='author']")[0].GetAttributeValue("content", String.Empty);
            a.Author = $"{author}";

            a.ImageUri = new Uri(doc.DocumentNode.SelectNodes("//meta[@property='og:image']")[0].GetAttributeValue("content", String.Empty));
            ImageCache.Add(new CachedUri(a.ImageUri));

            var avatarNode = doc.DocumentNode.SelectNodes("//a[@class='writers-link entry-header__author']/img");
            //if(avatarNode != null)
            //{
            //    a.AvatarUri = new Uri(avatarNode[0].GetAttributeValue("src", String.Empty));
            //    ImageCache.Add(new CachedUri(a.AvatarUri));
            //}
    
            var body = doc.DocumentNode.SelectNodes("//div[@class='entry-content']//p");
            StringBuilder lines = new StringBuilder();
            
            foreach (var p in body)
            {
                // Remove anchor tags
                String re = @"<a [^>]+>(.*?)<\/a>";
                string outputLine = Regex.Replace(p.OuterHtml, re, "$1");

                if(p.ParentNode.Name == "blockquote")
                {
                    lines.AppendLine("<p><b><i><center>" + outputLine.Replace("<p>", "").Replace("</p>", "") + "</center></i></b></p>");
                }
                else
                {
                    if(p.SelectNodes(".//img") == null && p.SelectNodes(".//iframe") == null) // Ignore inline images
                    {
                        if(lines.Length == 0)
                        {
                            // Display the dateline in the first paragraph
                            lines.AppendLine(outputLine.Replace("<p>", $"<p><b>{a.PublishDate}. </b>"));
                        }
                        else
                        {
                            lines.AppendLine(outputLine);
                        }
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
    
    private async Task<(Uri location, HttpResponseMessage httpResponse)> FetchPageAsync(Uri location)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Cookie", _config.SpecSessionCookie);
       
        var content = await client.GetAsync(location);
        return (location, content);
    }
}