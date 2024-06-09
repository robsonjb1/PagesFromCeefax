using System.Text;
using API.Architecture;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public interface IThurrottContent
{
    public List<ThurrottArticle> ThurrottArticles { get; set; }
}

public class ThurrottContent : IThurrottContent
{
    public List<ThurrottArticle> ThurrottArticles { get; set; } = new();
    private List<CachedUri> UrlCache { get; set; } = new();
    private List<CachedUri> ImageCache { get; set; } = new();
    private ISystemConfig _config;
 
    public ThurrottContent(ISystemConfig config)
    {
        _config = config;

        // Visit each article and retrieve the full details, including article and author images
        GetThurrottArticleList(@"https://www.thurrott.com/blog").Wait();
        GetThurrottArticleList(@"https://www.thurrott.com/blog/page/2").Wait();
     
        // Process the article URL cache
        Parallel.ForEach(UrlCache, u => u.ContentString = FetchPageAsync(u.Location).Result.httpResponse.Content.ReadAsStringAsync().Result);
        
        // Parse story content
        ThurrottArticles.ForEach(a => ReadThurrottArticle(a));

        // Process the image URL cache
        Parallel.ForEach(ImageCache, i => i.ContentBytes = FetchPageAsync(i.Location).Result.httpResponse.Content.ReadAsByteArrayAsync().Result);
        foreach(var a in ThurrottArticles)
        {
            if(a.ImageUri != null)
            {
                a.ImageBase64 = Convert.ToBase64String(ImageCache.Find(z => z.Location == a.ImageUri).ContentBytes);
            }
        }
    }

    private async Task GetThurrottArticleList(string root)
    {
        string rootHtml = await FetchPageAsync(new Uri(root)).Result.httpResponse.Content.ReadAsStringAsync();
        HtmlDocument doc = new();
        doc.LoadHtml(rootHtml);

        // Parse index page and construct article list
        var tags = doc.DocumentNode.SelectNodes("//section/h3/a/@href");
        if(tags != null)
        {
            foreach (var tag in tags)
            {
                string href = tag.Attributes["href"].Value;
                if(!href.Contains("/forums/"))
                {
                    var u = new Uri(href);

                    if(!ThurrottArticles.Exists(z=> z.ArticleUri == u))
                    {
                        ThurrottArticles.Add(new ThurrottArticle(u));
                        UrlCache.Add(new CachedUri(u));
                    }
                }
            }
        }
    }

    private void ReadThurrottArticle(ThurrottArticle a)
    {
        try
        {
            string articleHtml = UrlCache.Find(z => z.Location == a.ArticleUri).ContentString;
            HtmlDocument doc = new();
            doc.LoadHtml(articleHtml);

            a.Headline = doc.DocumentNode.SelectNodes("//meta[@property='og:title']")[0].GetAttributeValue("content", String.Empty);
            a.Byline = doc.DocumentNode.SelectNodes("//meta[@property='og:description']")[0].GetAttributeValue("content", String.Empty);
            a.Author = doc.DocumentNode.SelectNodes("//meta[@name='author']")[0].GetAttributeValue("content", String.Empty);
            a.ImageUri = new Uri(doc.DocumentNode.SelectNodes("//meta[@property='og:image']")[0].GetAttributeValue("content", String.Empty));
            ImageCache.Add(new CachedUri(a.ImageUri));
            
            DateTime publishDate = Convert.ToDateTime(doc.DocumentNode.SelectNodes("//meta[@property='article:published_time']")[0].GetAttributeValue("content", String.Empty));
            a.PublishDate = $"{publishDate.DayOfWeek}, {publishDate.Day}{Utility.GetDaySuffix(publishDate)} {publishDate:MMMM}";

            var body = doc.DocumentNode.SelectNodes("(//div[@class='thurrott-content-from-editor'])[1]/p");
            StringBuilder lines = new StringBuilder();
            
            foreach (var p in body)
            {
                if(p.SelectNodes(".//img") == null) // Ignore inline images
                {
                    if(lines.Length == 0)
                    {
                        // Display the dateline in the first paragraph
                        lines.AppendLine(p.OuterHtml.Replace("<p>", $"<p><b>{a.PublishDate}. </b>"));
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
    
    private async Task<(Uri location, HttpResponseMessage httpResponse)> FetchPageAsync(Uri location)
    {
        var client = new HttpClient();
        //client.DefaultRequestHeaders.Add("Cookie", _config.SpecSessionCookie);
       
        var content = await client.GetAsync(location);
        return (location, content);
    }
}