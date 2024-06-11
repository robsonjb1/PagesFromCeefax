using API.Architecture;
using HtmlAgilityPack;
using Serilog;

namespace API.Magazine;

public record ShareRecord
{
    public string Name { get; set; }
    public double Price { get; set; }
    public double Movement { get; set; }
}

public interface IShareData
{
    public List<ShareRecord> Shares { get; set; }
    public bool IsValid { get; set; }
}

public class ShareData : IShareData
{
    public List<ShareRecord> Shares { get; set; } = new();
    public bool IsValid { get; set; } = false;

    private readonly ICeefaxContent _cc;

    public ShareData(ICeefaxContent cc)
    {
        _cc = cc;

        try
        {
            ParseSharePage(CeefaxSectionType.SharesRising);
            ParseSharePage(CeefaxSectionType.SharesFalling);

            IsValid = true;
        }
        catch (OpenWeatherParseException ex)
        {
            Log.Fatal($"SHAREDATA BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
            Log.CloseAndFlush();
        }
    }

    private void ParseSharePage(CeefaxSectionType section)
    {
        string html = _cc.UriCache.First(l => l.Location == _cc.Sections.First(z => z.Name == section).Feed).ContentString;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
    }
}