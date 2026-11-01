using API.Architecture;

namespace API.Magazine;

public class NewsStory(CeefaxSectionType SectionName, string Headline, string Summary, DateTime PublishDate, Uri Link)
{
    public readonly CeefaxSectionType SectionName = SectionName;
    public readonly Uri Link = Link;
    public readonly string Headline = Headline;    
    public readonly string Summary = Summary;
    public readonly DateTime PublishDate = PublishDate;
    public bool Included = false;
    public bool New = (DateTime.UtcNow - PublishDate).TotalHours <= 1;
}

