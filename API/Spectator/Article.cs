namespace API.Spectator;

public class Article(Uri ArticleUri)
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

    public Uri ArticleUri { get; set; } = ArticleUri;
    public string Headline { get; set; } = String.Empty;
    public string Author { get; set; } = String.Empty;
    public Uri? AvatarUri { get;set; }
    public string AvatarBase64 { get; set; } = String.Empty;
    public string PublishDate { get; set; } = String.Empty;
    public string PublishTime { get; set; } = String.Empty;
    public Uri? ImageUri{ get; set; }
    public string StoryHtml { get; set; } = String.Empty;
    public string ImageBase64 { get; set; } = String.Empty;
    public bool IsValid {get; set; } = true;

#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

}
