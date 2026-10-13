using API.Architecture;

namespace API.Magazine;

public class NewsStory(CeefaxSectionType SectionName, string Headline, string Summary, DateTime PublishDate, Uri Link)
{
    public readonly CeefaxSectionType SectionName = SectionName;
    public readonly Uri Link = Link;
    public readonly List<string> Headline = Utility.ParseParagraph(Headline);
    public readonly List<string> Summary = Utility.ParseParagraph(Summary);
    public readonly DateTime PublishDate = PublishDate;
    public bool Included = false;
}

