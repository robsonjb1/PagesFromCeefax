using System.IO.Compression;
using System.Text;
using API.Magazine;

namespace API.PageGenerators;

public interface IThurrottNews
{
    public string Generate();
}

public class ThurrottNews : IThurrottNews
{
    private IThurrottContent _tc;
    public ThurrottNews(IThurrottContent tc)
    {
        _tc = tc;
    }

    public string Generate()
    {
        const int maxImageSize = 2*1024*1024; // 2mb limit on image sizes;

        // Header
        StringBuilder c = new StringBuilder();
        c.AppendLine("<html><head><style> .header_container {text-align: center;} img {display: block; margin:0 auto;} ");
        c.AppendLine("h2 {margin-top: 0px; margin-bottom: 0px;} .header_container p {margin-top: 5px; margin-bottom: 10px;} .headline_footer {font-size: small;} ");
        c.AppendLine(".body_container p:first-of-type {margin-top: 10px; margin-bottom: 10px;} </style></head><body>");

        c.AppendLine($"<div id='s0'><ol>");
        int count = 1;
        foreach(var article in _tc.ThurrottArticles)
        {
            if(article.IsValid)
            {
                c.AppendLine($"<li><a href='#s{count++}'>{article.Headline}</a><br><span class='headline_footer'>{article.Author}</span></li>");
            }
            else
            {   
                // Still list article for further investigation later
                c.AppendLine($"<li>ERROR! {article.Headline}<br>{article.Author}</li>");
            }
        }
        c.AppendLine("</ol><mbp:pagebreak />");

        // Article content
        count = 1;
        foreach(var article in _tc.ThurrottArticles.FindAll(z=>z.IsValid))
        {
            c.AppendLine($"<div id='s{count}'><div class='header_container'>");
            c.AppendLine($"<h2>{article.Headline}</h2>");  
            c.AppendLine($"<p><i>By {article.Author}. Skip to <a href='#s{count-1}'>previous</a> or <a href='#s{count+1}'>next</a>.</i></p></div>");
            
            if(article.ImageUri != null && article.ImageBase64 != String.Empty && article.ImageBase64.Length < maxImageSize)
            {
                c.AppendLine($"<img class='articleimage' src='data:image/{getMimeType(article.ImageUri)};base64,{article.ImageBase64}'>");
            }
            c.AppendLine($"<div class='body_container'>{article.StoryHtml}</div>");
            c.AppendLine("<a href='#s0'>Return to front page</a>");
            c.AppendLine("</div><mbp:pagebreak>");

            count++;
        }
      
        // File stats
        c.AppendLine($"File size: {c.Length.ToString("#,##0")} characters.<br>");
        c.AppendLine($"Local timestamp (UTC): {DateTime.UtcNow.ToString()}<br>");
        c.AppendLine("<br><a href='#s0'>Return to front page</a>");
        c.AppendLine("<mbp:pagebreak>");

        // Closing
        c.AppendLine("</div></body></html>");

        // Output file
        string filename = $"Thurrott {DateTime.Now.DayOfWeek} {DateTime.Now.Hour.ToString().PadLeft(2, '0')}{DateTime.Now.Minute.ToString().PadLeft(2, '0')}.htm";

        using (StreamWriter outputFile = new StreamWriter("KindleTemp/" + filename, false, Encoding.UTF8))
        {
            outputFile.WriteLine(c.ToString());
        }

        using (FileStream fs = new FileStream($"KindleTemp/{filename}.zip",FileMode.Create))
        using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
        {
            arch.CreateEntryFromFile($"KindleTemp/{filename}", filename);
        }       
        
        return $"KindleTemp/{filename}.zip";
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