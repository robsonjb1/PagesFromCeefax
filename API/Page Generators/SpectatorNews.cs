using System.IO.Compression;
using System.Net;
using System.Text;
using API.Magazine;
using HtmlAgilityPack;

namespace API.PageGenerators;

public interface ISpectatorNews
{
    public string Generate();
}

public class SpectatorNews(ISpectatorContent kc) : ISpectatorNews
{
    private const string EpubContentType = "application/epub+zip";
    private const string XhtmlContentType = "application/xhtml+xml";
    private readonly ISpectatorContent _kc = kc;

    public string Generate()
    {
        const int maxArticles = 50;
        string title = $"The Spectator - {DateTime.Now:dddd dd MMMM}";
        string filename = $"Spectator {DateTime.Now:dddd dd MMMM}.epub";
        string outputPath = Path.Combine("KindleTemp", filename);
        List<(string id, string href, string mediaType)> images = [];
        List<SpectatorArticle> articles = _kc.SpectatorArticles.FindAll(article => article.IsValid).Take(maxArticles).ToList();
        List<SpectatorCartoon> cartoons = _kc.SpectatorCartoons
            .Where(cartoon => cartoon.IsValid && !string.IsNullOrEmpty(cartoon.ImageBase64))
            .OrderBy(_ => Random.Shared.Next())
            .ToList();

        using (FileStream file = new(outputPath, FileMode.Create, FileAccess.ReadWrite))
        using (ZipArchive archive = new(file, ZipArchiveMode.Create))
        {
            WriteEntry(archive, "mimetype", EpubContentType, CompressionLevel.NoCompression);
            WriteEntry(archive, "META-INF/container.xml", """
                <?xml version="1.0" encoding="UTF-8"?>
                <container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
                  <rootfiles>
                    <rootfile full-path="OEBPS/package.opf" media-type="application/oebps-package+xml" />
                  </rootfiles>
                </container>
                """);

            string coverHref = AddImage(archive, images, "cover", _kc.CoverImageBase64, "image/jpeg", "cover.jpg");
            StringBuilder content = new();
            content.AppendLine("""
                <?xml version="1.0" encoding="UTF-8"?>
                <html xmlns="http://www.w3.org/1999/xhtml" xmlns:epub="http://www.idpf.org/2007/ops">
                <head>
                  <title>
                """);
            content.Append(WebUtility.HtmlEncode(title));
            content.AppendLine("</title>");
            content.AppendLine("""
                  <meta charset="utf-8" />
                  <style type="text/css">
                    body { font-family: serif; line-height: 1.4; }
                    h1, h2, .center { text-align: center; }
                    img { display: block; height: auto; margin: 1em auto; max-width: 100%; }
                    .avatar { max-width: 100px; }
                    .byline { font-style: italic; margin-top: 0; text-align: center; }
                    .page-break { break-before: page; page-break-before: always; }
                  </style>
                </head>
                <body>
                """);

            content.AppendLine("<section id=\"s0\">");
            if (!string.IsNullOrEmpty(coverHref))
            {
                content.Append("<img src=\"").Append(coverHref).AppendLine("\" alt=\"The Spectator cover\" />");
            }
            content.AppendLine("<h1>The Spectator</h1>");
            content.AppendLine("<nav epub:type=\"toc\" aria-label=\"Contents\"><h2>Contents</h2><ol>");
            for (int i = 0; i < articles.Count; i++)
            {
                content.Append("<li><a href=\"#s").Append(i + 1).Append("\">")
                    .Append(EncodeHeadline(articles[i].Headline))
                    .Append("</a><br /><span>")
                    .Append(WebUtility.HtmlEncode(articles[i].Author))
                    .AppendLine("</span></li>");
            }
            content.AppendLine("</ol></nav></section>");

            for (int i = 0; i < articles.Count; i++)
            {
                SpectatorArticle article = articles[i];
                int articleNumber = i + 1;
                content.Append("<section id=\"s").Append(articleNumber).AppendLine("\" class=\"page-break\">");
                content.AppendLine("<header>");

                if (!string.IsNullOrEmpty(article.AvatarBase64))
                {
                    string avatarType = GetImageMediaType(article.AvatarUri);
                    string avatarExtension = GetImageExtension(avatarType);
                    string avatarHref = AddImage(
                        archive, images, $"avatar-{articleNumber}", article.AvatarBase64, avatarType,
                        $"avatar-{articleNumber}.{avatarExtension}");
                    content.Append("<img class=\"avatar\" src=\"").Append(avatarHref)
                        .AppendLine("\" alt=\"\" />");
                }

                content.Append("<h2 style=\"margin-bottom: 0;\">").Append(EncodeHeadline(article.Headline)).AppendLine("</h2>");
                content.Append("<p class=\"byline\">By ").Append(WebUtility.HtmlEncode(article.Author))
                    .AppendLine("</p></header>");

                if (article.ImageUri != null && !string.IsNullOrEmpty(article.ImageBase64))
                {
                    string imageHref = AddImage(
                        archive, images, $"article-{articleNumber}", article.ImageBase64, "image/jpeg",
                        $"article-{articleNumber}.jpg");
                    content.Append("<img src=\"").Append(imageHref).Append("\" alt=\"")
                        .Append(EncodeHeadline(article.Headline)).AppendLine("\" />");
                }

                content.AppendLine("<div class=\"body_container\">");
                AppendStoryXhtml(content, article.StoryHtml);
                content.AppendLine("</div>");
                content.Append("<p class=\"byline\">By ").Append(WebUtility.HtmlEncode(article.Author))
                    .AppendLine("</p></section>");

                if (articleNumber % 2 == 0 && cartoons.Count > 0)
                {
                    SpectatorCartoon cartoon = cartoons[(articleNumber / 2) % cartoons.Count];
                    string cartoonHref = AddImage(
                        archive, images, $"cartoon-{articleNumber}", cartoon.ImageBase64, "image/jpeg",
                        $"cartoon-{articleNumber}.jpg");
                    content.Append("<section class=\"page-break center\"><img src=\"")
                        .Append(cartoonHref).Append("\" alt=\"")
                        .Append(WebUtility.HtmlEncode(cartoon.Caption)).AppendLine("\" />");
                    if (!string.IsNullOrEmpty(cartoon.Caption))
                    {
                        content.Append("<h2><i>").Append(WebUtility.HtmlEncode(cartoon.Caption)).AppendLine("</i></h2>");
                    }
                    content.AppendLine("</section>");
                }
            }

            content.AppendLine("<p class=\"page-break\">");
            content.Append("Generated ").Append(WebUtility.HtmlEncode(DateTime.UtcNow.ToString("u"))).AppendLine(" UTC.");
            content.AppendLine("</p></body></html>");

            WriteEntry(archive, "OEBPS/content.xhtml", content.ToString());
            WriteEntry(archive, "OEBPS/nav.xhtml", CreateNavigationDocument(title, articles));
            WriteEntry(archive, "OEBPS/package.opf", CreatePackageDocument(title, images));
        }

        return outputPath;
    }

    private static void AppendStoryXhtml(StringBuilder output, string storyHtml)
    {
        HtmlDocument document = new();
        document.LoadHtml($"<root>{storyHtml}</root>");
        foreach (HtmlNode node in document.DocumentNode.SelectSingleNode("/root").ChildNodes)
        {
            AppendXhtmlNode(output, node);
        }
    }

    private static void AppendXhtmlNode(StringBuilder output, HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Text)
        {
            output.Append(WebUtility.HtmlEncode(HtmlEntity.DeEntitize(node.InnerText)));
            return;
        }

        if (node.NodeType != HtmlNodeType.Element)
        {
            return;
        }

        string element = node.Name.ToLowerInvariant();
        if (element is not ("p" or "b" or "strong" or "i" or "em" or "u" or "sub" or "sup" or "br" or "ul" or "ol" or "li" or "blockquote" or "span"))
        {
            foreach (HtmlNode child in node.ChildNodes)
            {
                AppendXhtmlNode(output, child);
            }
            return;
        }

        output.Append('<').Append(element);
        if (element == "br")
        {
            output.Append(" />");
            return;
        }

        output.Append('>');
        foreach (HtmlNode child in node.ChildNodes)
        {
            AppendXhtmlNode(output, child);
        }
        output.Append("</").Append(element).Append('>');
    }

    private static string AddImage(
        ZipArchive archive,
        List<(string id, string href, string mediaType)> images,
        string id,
        string base64,
        string mediaType,
        string filename)
    {
        if (string.IsNullOrEmpty(base64))
        {
            return string.Empty;
        }

        string href = $"images/{filename}";
        ZipArchiveEntry entry = archive.CreateEntry($"OEBPS/{href}");
        using Stream stream = entry.Open();
        byte[] image = Convert.FromBase64String(base64);
        stream.Write(image, 0, image.Length);
        images.Add((id, href, mediaType));
        return href;
    }

    private static string CreateNavigationDocument(string title, List<SpectatorArticle> articles)
    {
        StringBuilder nav = new();
        nav.Append("""
            <?xml version="1.0" encoding="UTF-8"?>
            <html xmlns="http://www.w3.org/1999/xhtml" xmlns:epub="http://www.idpf.org/2007/ops">
            <head><title>
            """);
        nav.Append(WebUtility.HtmlEncode(title));
        nav.AppendLine("""
            </title><meta charset="utf-8" /></head>
            <body><nav epub:type="toc" id="toc"><h1>Contents</h1><ol>
            <li><a href="content.xhtml#s0">Cover and contents</a></li>
            """);
        for (int i = 0; i < articles.Count; i++)
        {
            nav.Append("<li><a href=\"content.xhtml#s").Append(i + 1).Append("\">")
                .Append(EncodeHeadline(articles[i].Headline))
                .AppendLine("</a></li>");
        }
        nav.AppendLine("</ol></nav></body></html>");
        return nav.ToString();
    }

    private static string CreatePackageDocument(string title, List<(string id, string href, string mediaType)> images)
    {
        StringBuilder package = new();
        package.Append("""
            <?xml version="1.0" encoding="UTF-8"?>
            <package xmlns="http://www.idpf.org/2007/opf" version="3.0" unique-identifier="book-id">
              <metadata xmlns:dc="http://purl.org/dc/elements/1.1/">
                <dc:identifier id="book-id">urn:uuid:
            """);
        package.Append(Guid.NewGuid());
        package.Append("</dc:identifier><dc:title>").Append(WebUtility.HtmlEncode(title))
            .AppendLine("""
                </dc:title><dc:language>en</dc:language>
                <meta property="dcterms:modified">
            """);
        package.Append(DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"));
        package.AppendLine("""
                </meta>
              </metadata>
              <manifest>
                <item id="content" href="content.xhtml" media-type="application/xhtml+xml" />
                <item id="nav" href="nav.xhtml" media-type="application/xhtml+xml" properties="nav" />
            """);
        foreach ((string id, string href, string mediaType) in images)
        {
            package.Append("<item id=\"").Append(id).Append("\" href=\"").Append(href)
                .Append("\" media-type=\"").Append(mediaType).Append('"');
            if (id == "cover")
            {
                package.Append(" properties=\"cover-image\"");
            }
            package.AppendLine(" />");
        }
        package.AppendLine("""
              </manifest>
              <spine><itemref idref="content" /></spine>
            </package>
            """);
        return package.ToString();
    }

    private static string GetImageMediaType(Uri imageUri)
    {
        return Path.GetExtension(imageUri?.AbsolutePath ?? string.Empty).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };
    }

    private static string EncodeHeadline(string headline)
    {
        return WebUtility.HtmlEncode(WebUtility.HtmlDecode(headline));
    }

    private static string GetImageExtension(string mediaType)
    {
        return mediaType switch
        {
            "image/png" => "png",
            "image/gif" => "gif",
            "image/bmp" => "bmp",
            "image/webp" => "webp",
            _ => "jpg"
        };
    }

    private static void WriteEntry(
        ZipArchive archive,
        string path,
        string content,
        CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        ZipArchiveEntry entry = archive.CreateEntry(path, compressionLevel);
        using StreamWriter writer = new(entry.Open(), new UTF8Encoding(false));
        writer.Write(content);
    }
}
