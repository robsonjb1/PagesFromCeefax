namespace API.Spectator;

public class Article(Uri ArticleUri)
{
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
    public bool IsValid = true;
}
