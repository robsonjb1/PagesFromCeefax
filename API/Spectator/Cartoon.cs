namespace API.Spectator;

public class Cartoon(Uri CartoonUri)
{
    public Uri CartoonUri { get; set; } = CartoonUri;
    public Uri? ImageUri { get; set; }
    public string ImageBase64 {get; set; } = String.Empty;
    public string Caption { get; set; } = String.Empty;
    public bool IsValid = true;
}
