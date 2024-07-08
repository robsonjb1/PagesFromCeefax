

using API.Architecture;

public interface IStreamingService
{
    public string StreamChannel(string channel);
}

// 1-1 The Way Back
// https://onedrive.live.com/embed?resid=6B5874908501F6E4%211026632&authkey=!AJioJZNBH-LeSkQ

// 1-2 Space Fall
// https://onedrive.live.com/embed?resid=6B5874908501F6E4%211026640&authkey=!ABSBTXNSxJsJBjA

// 1-3 Cygnus Alpha
// https://onedrive.live.com/embed?resid=6B5874908501F6E4%211026642&authkey=!AOtW963EiDftwT4

// 1-4 Time Squad
// https://onedrive.live.com/embed?resid=6B5874908501F6E4%211026641&authkey=!ABWKQ8xgUIGuzFg

// 1-5 The Web
// https://onedrive.live.com/embed?resid=6B5874908501F6E4%211026643&authkey=!AH-VmWK_dm_Jpt0

// 1-6 Seek, Locate, Destroy
// https://onedrive.live.com/embed?resid=6B5874908501F6E4%211026645&authkey=!ALWgTs15ki1XJ1E

// 1-7 Mission to Destiny
// https://onedrive.live.com/embed?resid=6B5874908501F6E4%211026644&authkey=!AFq61TOifbFDwNo

// 1-8 Duel
// https://onedrive.live.com/embed?resid=6B5874908501F6E4%211026646&authkey=!ANcHcbOEygyp-hs

// 1-9 Project Avalon
// https://onedrive.live.com/embed?resid=6B5874908501F6E4%211026648&authkey=!AOzR7vEul49NO54


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

    public StreamingService()
    {
        string listings = File.ReadAllText("./wwwroot/tv/listings.dat");
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
        DateTime dt = Utility.ConvertToUKTime(DateTime.UtcNow);

        int totalItems = _media.Count();
        int selectedItem = new Random(dt.Month + dt.Day + dt.Hour).Next(totalItems);

        return _media[selectedItem].Url;
    }
}