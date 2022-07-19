namespace API.Magazine
{
    public class CachedUrl
    {
        public Uri Location { get; }
        public string Content { get; set; }

        public CachedUrl(Uri location) => this.Location = location;
    }
}