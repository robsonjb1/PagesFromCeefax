namespace API.Magazine;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

public class ThurrottArticle(Uri ArticleUri)
{
    public Uri ArticleUri { get; set; } = ArticleUri;
    public string Headline { get; set; } = String.Empty;
    public string Byline {get; set; } = String.Empty;
    public string Author { get; set; } = String.Empty;
    public string PublishDate { get; set; } = String.Empty;
    public Uri? ImageUri{ get; set; }
    public string StoryHtml { get; set; } = String.Empty;
    public string ImageBase64 { get; set; } = String.Empty;
    public bool IsValid {get; set; } = false;

}
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

