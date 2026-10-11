namespace API.Architecture;

public class CachedUri(Uri location, string key = "")
{
    public Uri Location { get; } = location;
    public string Tag { get; set; } = key;
    public string ContentString { get; set; }
    public byte[] ContentBytes { get; set; }
}