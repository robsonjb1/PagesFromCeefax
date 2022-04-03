using System;
using HtmlAgilityPack;

namespace PagesFromCeefax
{
    public class CachedUrl
    {
        public Uri Location { get; set; }
        public string Content { get; set; } 

        public CachedUrl(Uri Location, string Content = "")
        {
            this.Location = Location;
            this.Content = Content;
        }
    }
}