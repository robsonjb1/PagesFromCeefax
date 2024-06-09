namespace API.Architecture;

public class CachedUri
{
    public Uri Location { get; }
    public string ContentString { get; set; }
    public byte[] ContentBytes { get; set; }
    public CachedUri(Uri location) => this.Location = location;
}