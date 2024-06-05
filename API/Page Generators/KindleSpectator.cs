using System.IO.Compression;
using System.Text;
using API.Magazine;

namespace API.PageGenerators;

public interface IKindleSpectator
{
    public string Generate();
}

public class KindleSpectator : IKindleSpectator
{
    private IKindleContent _kc;
    public KindleSpectator(IKindleContent kc)
    {
        _kc = kc;
    }

    public string Generate()
    {
        const int maxArticles = 50;
        const int maxCartoons = 10;
        
        // Heading
        StringBuilder m = new StringBuilder();
        m.AppendLine("<html><head><style> .header_container {text-align: center;} img {display: block; margin:0 auto;} ");
        m.AppendLine("h2 {margin-top: 0px; margin-bottom: 0px;} .header_container p {margin-top: 5px; margin-bottom: 10px;} ");
        m.AppendLine(".body_container p:first-of-type {margin-top: 10px; margin-bottom: 10px;} </style></head>");
       
        m.AppendLine("<body><div id='0'>");
        m.AppendLine($"<center><img width='100%' src='data:image/png;base64,{_kc.SpectatorLogoBase64}'>");
        
        // Build index
        m.AppendLine($"</center><ol>");
        int count = 1;
        foreach(var article in _kc.SpectatorArticles.FindAll(z=>z.IsValid).Take(maxArticles))
        {
            m.AppendLine($"<li><a href='#{count++}'>{article.Headline}</a><br>{article.Author}</li>");
        }
        m.AppendLine($"<li><a href='#{count++}'>Cartoons</a></li>");
        m.AppendLine("</ol><mbp:pagebreak />");
        
        // Build content
        StringBuilder c = new StringBuilder();

        count = 1;
        foreach(var article in _kc.SpectatorArticles.FindAll(z=>z.IsValid).Take(maxArticles))
        {
            c.AppendLine($"<div id='{count}'><div class='header_container'>");
            //c.AppendLine($"<p><i>{article.PublishDate}, {article.PublishTime}<br>");
            if(article.AvatarBase64 != String.Empty)
            {
                c.AppendLine($"<img src='data:image/png;base64,{article.AvatarBase64}'>");
            }        
            c.AppendLine($"<h2>{article.Headline}</h2>");  
            c.AppendLine($"<p><i>By {article.Author}. Skip to <a href='#{count-1}'>previous</a> or <a href='#{count+1}'>next</a>.</i></p></div>");
            
            if(article.ImageUri != null && article.ImageBase64 != String.Empty)
            {
                c.AppendLine($"<img class='articleimage' src='data:image/{getMimeType(article.ImageUri)};base64,{article.ImageBase64}'>");
            }
            c.AppendLine($"<div class='body_container'>{article.StoryHtml}</div>");
            c.AppendLine("<a href='#0'>Return to front page</a>");
            c.AppendLine("</div><mbp:pagebreak>");

            count++;
        }

        // Cartoons
        foreach(var cartoon in _kc.SpectatorCartoons.FindAll(z=>z.IsValid).Take(maxCartoons))
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
}