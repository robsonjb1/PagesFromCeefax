namespace API.Magazine;

public class RegisterArticle(Uri ArticleUri)
{
    public Uri ArticleUri { get; set; } = ArticleUri;
    public string Headline { get; set; } = String.Empty;
    public string Byline { get; set; } = String.Empty;
    public string Section { get; set; } = String.Empty;
    public string StoryHtml { get; set; } = String.Empty;
    public DateTime PublishDate { get; set; } 
    public bool IsValid = true;
}
