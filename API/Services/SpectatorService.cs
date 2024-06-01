// See https://aka.ms/new-console-template for more information

using HtmlAgilityPack;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.IO.Compression;
using API.Architecture;
using API.Spectator;
using System.Diagnostics;

namespace API.Services;

public interface ISpectatorService
{
    public Task<string> Spectator();
}

public class SpectatorService : ISpectatorService
{
    private readonly ISystemConfig _config;

    public SpectatorService(ISystemConfig config)
    {
        _config = config;
    }

    public async Task<string> Spectator()
    {
        try
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            // Visit each article and retrieve the full details, including article and author images
            List<Article> articles = await GetArticleList(@"https://www.spectator.co.uk/");
            articles.ForEach(a => ReadArticle(a));

            // Visit each cartoon and retrieve the full details
            List<Cartoon> cartoons = await GetCartoonList(@"https://www.spectator.co.uk/illustrations/");
            cartoons.ForEach(c => ReadCartoon(c));

            // Construct final html file
            string filename = BuildMagazine(articles, cartoons);

            // Send e-mail to Kindle
            SendEMail(filename);
            
            return $"Done in {Math.Round(s.ElapsedMilliseconds / 1000d)} seconds.";
        }
        catch (Exception ex)
        {
            return $"{ex.Message} {ex.InnerException} {ex.StackTrace}";
        }
    }
    private async Task<List<Article>> GetArticleList(string specRoot)
    {
        string rootHtml = await FetchPageAsync(new Uri(specRoot)).Result.httpResponse.Content.ReadAsStringAsync();
        List<Article> articles = new List<Article>();

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
                    if(!articles.Exists(z=> z.ArticleUri == u))
                    {
                        articles.Add(new Article(u));
                    }
                }
            }
        }

        return articles;
    }

    private async Task<List<Cartoon>> GetCartoonList(string specRoot)
    {
        string rootHtml = await FetchPageAsync(new Uri(specRoot)).Result.httpResponse.Content.ReadAsStringAsync();
        List<Cartoon> cartoons = new List<Cartoon>();

        HtmlDocument doc = new();
        doc.LoadHtml(rootHtml);

        // Parse index page and construct article list
        var tags = doc.DocumentNode.SelectNodes("//a[@class='article__title-link']");
        if(tags != null)
        {
            foreach (var tag in tags)
            {
                string href = tag.Attributes["href"].Value;
                cartoons.Add(new Cartoon(new Uri(href)));
            }
        }

        return cartoons;
    }

    private async void ReadArticle(Article a)
    {
        try
        {
            string articleHtml = await FetchPageAsync(a.ArticleUri).Result.httpResponse.Content.ReadAsStringAsync();
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
                    lines.AppendLine("<b><i><center>" + p.OuterHtml + "</center></i></b>");
                }
                else
                {
                    lines.AppendLine(p.OuterHtml);
                }
            }
            a.StoryHtml = lines.ToString();      
        }
        catch
        {
            a.IsValid = false;
        }
    }

    private async void ReadCartoon(Cartoon c)
    {
        try
        {
            string cartoonHtml = await FetchPageAsync(c.CartoonUri).Result.httpResponse.Content.ReadAsStringAsync();
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

    private async Task<(Uri location, HttpResponseMessage httpResponse)> FetchPageAsync(Uri location)
    {
        var client = new HttpClient();
        var content = await client.GetAsync(location);
        return (location, content);
    }

    private string BuildMagazine(List<Article> articles, List<Cartoon> cartoons)
    {
        StringBuilder m = new StringBuilder();
        int maxArticles = 50;
        int maxCartoons = 10;

        // Heading
        m.AppendLine("<html><head><style> .container {text-align: center;} img {display: block; margin:0 auto;} h2 {margin-top: 0px; margin-bottom: 0px;} p {margin-top: 5px; margin-bottom: 10px;}</style></head>");
        m.AppendLine("<body><div id='0'>");

        string logo = Task.Run(async () => await ImageUrlToBase64(new Uri("https://logos-download.com/wp-content/uploads/2016/10/The_Spectator_logo_text_wordmark.png"))).Result;
        m.AppendLine($"<center><img width='100%' src='data:image/png;base64,{logo}'>");
        
        // Build index
        m.AppendLine($"<h1>{DateTime.Now.DayOfWeek}</h1></center><ol>");
        int count = 1;
        foreach(var article in articles.FindAll(z=>z.IsValid).Take(maxArticles))
        {
            m.AppendLine($"<li><a href='#{count++}'>{article.Headline}</a><br>{article.Author}</li>");
        }
        m.AppendLine($"<li><a href='#{count++}'>Cartoons</a></li>");
        m.AppendLine("</ol><mbp:pagebreak />");
        
        // Build content
        StringBuilder c = new StringBuilder();

        count = 1;
        foreach(var article in articles.FindAll(z=>z.IsValid).Take(maxArticles))
        {
            c.AppendLine($"<div id='{count}'><div class='container'>");
            //c.AppendLine($"<p><i>{article.PublishDate}, {article.PublishTime}<br>");
            if(article.AvatarBase64 != String.Empty)
            {
                c.AppendLine($"<img src='data:image/png;base64,{article.AvatarBase64}'>");
            }        
            c.AppendLine($"<h2>{article.Headline}</h2>");  
            c.AppendLine($"<p><i>By {article.Author}. Skip to <a href='#{count-1}'>previous</a> or <a href='#{count+1}'>next</a>.</i></p></div>");
            
            if(article.ImageUri != null && article.ImageBase64 != String.Empty)
            {
                c.AppendLine($"<img class='articleimage' src='data:image/{getMimeType(article.ImageUri)};base64,{article.ImageBase64}'><br>");
            }
            c.AppendLine(article.StoryHtml);
            c.AppendLine("<a href='#0'>Return to front page</a>");
            c.AppendLine("</div><mbp:pagebreak>");

            count++;
        }

        // Cartoons
        foreach(var cartoon in cartoons.FindAll(z=>z.IsValid).Take(maxCartoons))
        {
            c.AppendLine($"<div id='{count}'><br><br><center>");
            c.AppendLine($"<img src='data:image/{getMimeType(cartoon.CartoonUri)};base64,{cartoon.ImageBase64}'>");
            c.AppendLine($"<h2><i>{cartoon.Caption}</i></h2>");
            c.AppendLine("</center></div><mbp:pagebreak>");
        }

        // File stats
        c.AppendLine($"File size: {(m.Length + c.Length).ToString("#,##0")} characters.<br>");
        c.AppendLine($"Local timestamp (UTC): {DateTime.UtcNow.ToString()}<br>");
        c.AppendLine("<br><a href='#0'>Return to front page</a>");
        c.AppendLine("<mbp:pagebreak>");

        // Output file
        string filename = $"Spectator {DateTime.Now.DayOfWeek}.htm";

        if (Directory.Exists("spectemp")) { Directory.Delete("spectemp", true); }
        Directory.CreateDirectory("spectemp");
        
        if (Directory.Exists("speczip")) { Directory.Delete("speczip", true); }
        Directory.CreateDirectory("speczip");

        using (StreamWriter outputFile = new StreamWriter("spectemp/" + filename, false, Encoding.UTF8))
        {
            outputFile.WriteLine(m.ToString() + c.ToString() + "</div></body></html>");
        }

        if(File.Exists($"speczip/{filename}.zip")) { File.Delete($"speczip/{filename}.zip"); }
        ZipFile.CreateFromDirectory("spectemp", $"speczip/{filename}.zip");
        
        return $"speczip/{filename}.zip";
    }

    private string getMimeType(Uri imageUri)
    {
        switch(imageUri.AbsoluteUri.Substring(imageUri.AbsoluteUri.Length - 3))
        {
            case "png":
                return "png";
            case "gif":
                return "gif";
            case "bmp":
                return "bmp";
            default:
                return "jpeg";
        }
    }

    public async Task<string> ImageUrlToBase64(Uri imageUri)
    {
        using var httpClient = new HttpClient();
        var imageBytes = await httpClient.GetByteArrayAsync(imageUri);                
        return Convert.ToBase64String(imageBytes);
    }

    private void SendEMail(string filename)
    {
        var smtp = new SmtpClient
        {
            Host = _config.SpecHost,
            Port = _config.SpecPort,
            EnableSsl = _config.SpecEnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_config.SpecFromUsername, _config.SpecFromPassword)
        };

        Attachment attachment = new Attachment(filename);
        using (var message = new MailMessage(
            new MailAddress(_config.SpecFromAddress, _config.SpecName),
            new MailAddress(_config.SpecToAddress, _config.SpecName)
        )
        {
            Subject = "Spectator upload",
            Body = ""
        })
        {
            message.Attachments.Add(attachment);
            smtp.Send(message);
        }
    }   
}