using API.Architecture;

public interface IStreamingService
{
    public string StreamChannel(string channel);
}

public class StreamingService : IStreamingService
{
    public class StreamingMedia {
        public string Title { get; }
        public string Url { get; }

        public StreamingMedia(string title, string url)
        {
            Title = title;
            Url = url;
        }
    }

    private List<StreamingMedia> _media = [];
    private ISystemConfig _config;

    public StreamingService(ISystemConfig c)
    {
        _config = c;

        string listings = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "listings.txt"));
        bool first = true;
        string name = String.Empty;
        foreach (string entry in listings.Split(Environment.NewLine))
        {
            if(first)
            {
                name = entry;
            }
            else
            {
                _media.Add(new StreamingMedia(name, entry));
            }
            first = !first;
        }
    }
    public string StreamChannel(string channel)
    {
        //if(channel == _config.StreamingCode)
        //{
            DateTime dt = Utility.ConvertToUKTime(DateTime.UtcNow);

            int totalItems = _media.Count();
            int selectedItem = new Random(dt.Month * dt.Day * dt.Hour).Next(totalItems);

            return _media[selectedItem].Url;
        //}
        //else
        //{
        //    return String.Empty;
        //}
    }
}