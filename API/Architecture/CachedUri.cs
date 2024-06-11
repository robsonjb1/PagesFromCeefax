namespace API.Architecture;

public class CachedUri
{
    public Uri Location { get; }
    public string Tag { get; set; } = String.Empty;
    public string ContentString { get; set; }
    public byte[] ContentBytes { get; set; }
    public CachedUri(Uri location, string key = "")
    {
        this.Location = location;   
        this.Tag = key; 
    }
}