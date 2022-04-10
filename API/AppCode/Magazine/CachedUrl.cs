using System;
using HtmlAgilityPack;

namespace PagesFromCeefax
{
    public class CachedUrl
    {
        public Uri? Location { get; set; }
        public string? Content { get; set; }
    }
}