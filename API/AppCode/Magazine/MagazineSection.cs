using System;

namespace PagesFromCeefax
{
    public class MagazineSection
    {
        public readonly MagazineSectionType Name;
        public readonly Uri Feed;
        public readonly int TotalStories;
        public readonly string Keyword;


        public MagazineSection(MagazineSectionType Name, int TotalStories, Uri Feed, string Keyword = "")
        {
            this.Name = Name;
            this.Feed = Feed;
            this.TotalStories = TotalStories;
            this.Keyword = Keyword;
        }

    }
}