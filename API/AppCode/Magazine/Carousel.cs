using System;
using System.Text;

namespace PagesFromCeefax
{
    public class Carousel
    {
        public readonly int PageNo = 0;
        public readonly StringBuilder Content = new StringBuilder();
        
        public Carousel()
        {
            // Get and parse source data
            MagazineContent mc = new MagazineContent();
            WeatherData weather = new WeatherData(
                mc.UrlCache.Find(l => l.Location == mc.Sections.Find(z => z.Name == MagazineSectionType.Weather).Feed).Content);

            // Initialise page generators
            TeletextPageNews tn = new TeletextPageNews(mc);
            TeletextPageWeather tw = new TeletextPageWeather(mc);

            // This is teletext
            Content.Append(tn.BuildTeletextPage(ref PageNo, Graphics.PromoTeletext));
            Content.Append(tn.BuildTeletextPage(ref PageNo, Graphics.PromoNews));

            // Home/World/Politics/SciTech/Business news
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderHome, Mode7Colour.Yellow, MagazineSectionType.Home, "", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsInBrief(ref PageNo, Graphics.HeaderHome, Mode7Colour.Yellow, MagazineSectionType.Home, "World news coming up next >>>", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderWorld, Mode7Colour.Yellow, MagazineSectionType.World, "", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsInBrief(ref PageNo, Graphics.HeaderWorld, Mode7Colour.Yellow, MagazineSectionType.World, "Political news coming up next >>>", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderPolitics, Mode7Colour.Yellow, MagazineSectionType.Politics, "Technology news coming up next >>>", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderSciTech, Mode7Colour.Yellow, MagazineSectionType.Technology, "Sussex news coming up next >>>", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderSussex, Mode7Colour.Yellow, MagazineSectionType.Sussex, "Business news coming up next >>>", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderBusiness, Mode7Colour.Yellow, MagazineSectionType.Business, "", Mode7Colour.Red, Mode7Colour.White));
            Content.Append(tn.OutputNewsInBrief(ref PageNo, Graphics.HeaderBusiness, Mode7Colour.Yellow, MagazineSectionType.Business, "", Mode7Colour.Red, Mode7Colour.White));

            // Sports Intro
            Content.Append(tn.BuildTeletextPage(ref PageNo, Graphics.PromoSport));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderFootball, Mode7Colour.Green, MagazineSectionType.Football, "", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsInBrief(ref PageNo, Graphics.HeaderFootball, Mode7Colour.Green, MagazineSectionType.Football, "", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderRugby, Mode7Colour.Green, MagazineSectionType.Rugby, "Cricket news coming up next >>>", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderCricket, Mode7Colour.Green, MagazineSectionType.Cricket, "Tennis news coming up next >>>", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderTennis, Mode7Colour.Green, MagazineSectionType.Tennis, "Golf news coming up next >>>", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderGolf, Mode7Colour.Green, MagazineSectionType.Golf, "Motorsport news coming up next >>>", Mode7Colour.Blue, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderFormula1, Mode7Colour.Green, MagazineSectionType.Formula1, "", Mode7Colour.Blue, Mode7Colour.Yellow));

            // Weather Intro
            Content.Append(tn.BuildTeletextPage(ref PageNo, Graphics.PromoWeather));
            Content.Append(tw.OutputWeather(ref PageNo, 1, weather.TodayTitle, weather.TodayText));
            Content.Append(tw.OutputWeather(ref PageNo, 2, weather.TomorrowTitle, weather.TomorrowText));
            Content.Append(tw.OutputWeather(ref PageNo, 3, weather.OutlookTitle, weather.OutlookText));
            
            // Entertainment Intro
            Content.Append(tn.BuildTeletextPage(ref PageNo, Graphics.PromoTV));
            Content.Append(tn.OutputNewsSection(ref PageNo, Graphics.HeaderEntertainment, Mode7Colour.Yellow, MagazineSectionType.Entertainment, "", Mode7Colour.Magenta, Mode7Colour.Yellow));
            Content.Append(tn.OutputNewsInBrief(ref PageNo, Graphics.HeaderEntertainment, Mode7Colour.Yellow, MagazineSectionType.Entertainment, "", Mode7Colour.Magenta, Mode7Colour.Yellow));
            
            // Close
            Content.Append(tn.BuildTeletextPage(ref PageNo, Graphics.PromoLinks));

            Content.AppendLine("<!-- Service started: {PFC_SERVICESTART} -->");
            Content.AppendLine("<!-- Carousel built at: {PFC_TIMESTAMP} -->");
            Content.AppendLine("<!-- Total Requests: {PFC_TOTALREQUESTS} -->");
            Content.AppendLine("<!-- Total Carousels: {PFC_TOTALCAROUSELS} -->");
            
            Content.Append("<div id='totalPages' style='display:none'>" + PageNo + "</div>");
        }
    }
}