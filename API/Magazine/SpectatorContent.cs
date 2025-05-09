using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using API.Architecture;
using HtmlAgilityPack;
using Serilog;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;

namespace API.Magazine;

public interface ISpectatorContent
{
    public List<SpectatorArticle> SpectatorArticles { get; set; }
    public List<SpectatorCartoon> SpectatorCartoons { get; set; }
    public string CoverImageBase64 { get; set; }
}

public class SpectatorContent : ISpectatorContent
{
    public List<SpectatorArticle> SpectatorArticles { get; set; } = new();
    public List<SpectatorCartoon> SpectatorCartoons { get; set; } = new();
    public string CoverImageBase64 { get; set; } = String.Empty;
    private List<CachedUri> UrlCache { get; set; } = new();
    private List<CachedUri> ImageCache { get; set; } = new();
    private ISystemConfig _config;
 
    public SpectatorContent(ISystemConfig config)
    {
        _config = config;

        // Visit each article and retrieve the full details, including article and author images
        GetSpectatorArticleList(@"https://www.spectator.co.uk/").Wait();
     
        // Visit each cartoon and retrieve the full details
        GetCartoonList(@"https://www.spectator.co.uk/illustrations/").Wait();

        // Process the URL article cache (deliberately do this sequentially)
        UrlCache.ForEach(u => u.ContentString = FetchPageAsync(u.Location).Result.httpResponse.Content.ReadAsStringAsync().Result);
        
        // Parse story content
        SpectatorArticles.ForEach(a => ReadSpectatorArticle(a));
        SpectatorCartoons.ForEach(c => ReadSpectatorCartoon(c));

        // Process the URL image cache
        Parallel.ForEach(ImageCache, i => i.ContentBytes = FetchPageAsync(i.Location).Result.httpResponse.Content.ReadAsByteArrayAsync().Result);
        foreach(var a in SpectatorArticles)
        {
            if(a.ImageUri != null)
            {
                a.ImageBase64 = ReduceImageQuality(ImageCache.Find(z => z.Location == a.ImageUri).ContentBytes);             
            }

            if(a.AvatarUri != null)
            {
                a.AvatarBase64 = Convert.ToBase64String(ImageCache.Find(z => z.Location == a.AvatarUri).ContentBytes);
            }
        }

        foreach(var c in SpectatorCartoons)
        {
            if(c.ImageUri != null)
            {
                c.ImageBase64 = ReduceImageQuality(ImageCache.Find(z => z.Location == c.ImageUri).ContentBytes);
            }
        }
        

        // Attempt to retrieve the current magazine cover image
        DateTime coverSaturday;
        DateTime now = DateTime.Now;
        switch(now.DayOfWeek)
        {
            case DayOfWeek.Sunday:
                coverSaturday = now.AddDays(-1);
                break;
            case DayOfWeek.Monday:
                coverSaturday = now.AddDays(-2);
                break;
            case DayOfWeek.Tuesday:
                coverSaturday = now.AddDays(-3);
                break;
            case DayOfWeek.Wednesday:
                coverSaturday = now.AddDays(-4);
                break;
            case DayOfWeek.Thursday:
                coverSaturday = now.AddDays(2);
                break;
            case DayOfWeek.Friday:
                coverSaturday = now.AddDays(1);
                break;
            default:
                coverSaturday = now;
                break;
        }

        string baseUrl = String.Format("https://www.spectator.co.uk/wp-content/uploads/{0}/cover-{1}-issue.jpg",
          coverSaturday.AddDays(-2).Year + "/" + coverSaturday.AddDays(-3).Month.ToString("00"), // The date on which the image would have been uploaded, ie the previous Wednesday
          coverSaturday.Day.ToString("00") + coverSaturday.Month.ToString("00") + coverSaturday.Year
        );

        CoverImageBase64 = ReduceImageQuality(FetchPageAsync(new Uri(baseUrl)).Result.httpResponse.Content.ReadAsByteArrayAsync().Result);
    }

    private string ReduceImageQuality(byte[] source)
    {
        try
        {
            // Reduce image quality
            using var image = SixLabors.ImageSharp.Image.Load(source);
            image.Mutate(z => z.Brightness(1.2f));
            using MemoryStream outputStream = new MemoryStream();
            image.Save(outputStream, new JpegEncoder { Quality = 50 });
            outputStream.Close();

            return Convert.ToBase64String(outputStream.ToArray());  
        }
        catch
        {
            return String.Empty;
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
                // Strip off any params
                if(href.Contains('/') && !href.EndsWith('/')) {
                    href = href[..(href.LastIndexOf('/')+1)];
                }
                if(href.Contains("/article/")) {
                    Uri u = new Uri(href);
                    if(!SpectatorArticles.Exists(z=> z.ArticleUri == u))
                    {
                        Debug.Print(href);
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

                // Remove Unicode emoji characters
                outputLine = Regex.Replace(outputLine, @"[^\u0000-\u9999]+", string.Empty);

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
    
    private void ReadSpectatorCartoon(SpectatorCartoon c)
    {
        try
        {
            string cartoonHtml = UrlCache.Find(z => z.Location == c.CartoonUri).ContentString;
            HtmlDocument doc = new();
            doc.LoadHtml(cartoonHtml);
            c.Caption = doc.DocumentNode.SelectNodes("//meta[@property='og:description']")[0].GetAttributeValue("content", String.Empty);
            c.Caption = c.Caption.StartsWith("Weekly magazine") ? String.Empty : c.Caption;
            c.ImageUri = new Uri(doc.DocumentNode.SelectNodes("//meta[@property='og:image']")[0].GetAttributeValue("content", String.Empty));
            ImageCache.Add(new CachedUri(c.ImageUri));
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
                UrlCache.Add(new CachedUri(u));
            }
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
