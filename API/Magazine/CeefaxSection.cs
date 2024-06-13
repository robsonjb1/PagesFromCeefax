using System.Text;
using API.Architecture;

namespace API.Magazine;

public class CeefaxSection
{
    public readonly CeefaxSectionType Name;
    public readonly Uri Feed;
    public readonly int TotalStories;
    public readonly bool HasNewsInBrief = false;
    public readonly StringBuilder Header;
    public readonly Mode7Colour HeadingCol;
    public readonly string PromoFooter;
    public readonly Mode7Colour PromoPaper;
    public readonly Mode7Colour PromoInk;

    public CeefaxSection(CeefaxSectionType Name, Uri Feed)
    {
        this.Name = Name;
        this.Feed = Feed;
       
        // Initialise the visual parameters depending on section
        switch (Name)
        {
            case CeefaxSectionType.Home:
            case CeefaxSectionType.World:
            case CeefaxSectionType.Politics:
            case CeefaxSectionType.Science:
            case CeefaxSectionType.Technology:
            case CeefaxSectionType.Sussex:
            case CeefaxSectionType.TVScheduleBBC1:
            case CeefaxSectionType.TVScheduleBBC2:
            case CeefaxSectionType.TVScheduleBBC4:
            case CeefaxSectionType.Weather:
            case CeefaxSectionType.WeatherForecast1:
            case CeefaxSectionType.WeatherForecast2:
            case CeefaxSectionType.WeatherForecast3:
            case CeefaxSectionType.WeatherWorld:
                this.HeadingCol = Mode7Colour.Yellow;
                this.PromoPaper = Mode7Colour.Blue;
                this.PromoInk = Mode7Colour.Yellow;
                break;

            case CeefaxSectionType.Business:
            case CeefaxSectionType.Markets:
                this.HeadingCol = Mode7Colour.Yellow;
                this.PromoPaper = Mode7Colour.Red;
                this.PromoInk = Mode7Colour.White;
                break;

            case CeefaxSectionType.Football:
            case CeefaxSectionType.Rugby:
            case CeefaxSectionType.Cricket:
            case CeefaxSectionType.Tennis:
            case CeefaxSectionType.Golf:
            case CeefaxSectionType.Formula1:
            case CeefaxSectionType.Standings:
                this.HeadingCol = Mode7Colour.Green;
                this.PromoPaper = Mode7Colour.Blue;
                this.PromoInk = Mode7Colour.Yellow;
                break;

            case CeefaxSectionType.Entertainment:
                this.HeadingCol = Mode7Colour.Yellow;
                this.PromoPaper = Mode7Colour.Magenta;
                this.PromoInk = Mode7Colour.Yellow;
                break;

            default:
                break;
        }

        // Initialise the full section sizes
        switch (Name)
        {
            case CeefaxSectionType.Home:
            case CeefaxSectionType.World:
            case CeefaxSectionType.Politics:
            case CeefaxSectionType.Business:
            case CeefaxSectionType.Football:
            case CeefaxSectionType.Entertainment:
                this.TotalStories = 3;
                this.HasNewsInBrief = true;
                break;

            case CeefaxSectionType.Markets:
            case CeefaxSectionType.Standings:
            case CeefaxSectionType.Weather:
            case CeefaxSectionType.WeatherForecast1:
            case CeefaxSectionType.WeatherForecast2:
            case CeefaxSectionType.WeatherForecast3:
            case CeefaxSectionType.WeatherWorld:
            case CeefaxSectionType.TVScheduleBBC1:
            case CeefaxSectionType.TVScheduleBBC2:
            case CeefaxSectionType.TVScheduleBBC4:
                this.TotalStories = 0;
                break;

            default:
                this.TotalStories = 2;
                break;
        }

        // Initialise header banner and promo footer text
        switch (Name)
        {
            case CeefaxSectionType.Home:
                this.Header = Graphics.HeaderHome;
                this.PromoFooter = "World news coming up next >>>";
                break;
            case CeefaxSectionType.World:
                this.Header = Graphics.HeaderWorld;
                this.PromoFooter = "Political news coming up next >>>";
                break;
            case CeefaxSectionType.Politics:
                this.Header = Graphics.HeaderPolitics;
                this.PromoFooter = "Technology news coming up next >>>";
                break;
            case CeefaxSectionType.Science:
                this.Header = Graphics.HeaderSciTech;
                break;
            case CeefaxSectionType.Technology:
                this.Header = Graphics.HeaderSciTech;
                this.PromoFooter = "Sussex news coming up next >>>";
                break;
            case CeefaxSectionType.Sussex:
                this.Header = Graphics.HeaderSussex;
                this.PromoFooter = "Business news coming up next >>>";
                break;
            case CeefaxSectionType.Business:
                this.Header = Graphics.HeaderBusiness;
                this.PromoFooter = "Market data coming up next >>>";
                break;
            case CeefaxSectionType.Football:
                this.Header = Graphics.HeaderFootball;
                this.PromoFooter = "Rugby news coming up next >>>";
                break;
            case CeefaxSectionType.Rugby:
                this.Header = Graphics.HeaderRugby;
                this.PromoFooter = "Cricket news coming up next >>>";
                break;
            case CeefaxSectionType.Cricket:
                this.Header = Graphics.HeaderCricket;
                this.PromoFooter = "Tennis news coming up next >>>";
                break;
            case CeefaxSectionType.Tennis:
                this.Header = Graphics.HeaderTennis;
                this.PromoFooter = "Golf news coming up next >>>";
                break;
            case CeefaxSectionType.Golf:
                this.Header = Graphics.HeaderGolf;
                this.PromoFooter = "Motorsport news coming up next >>>";
                break;
            case CeefaxSectionType.Formula1:
                this.Header = Graphics.HeaderFormula1;
                break;
            case CeefaxSectionType.TVScheduleBBC4:
                this.Header = Graphics.HeaderGolf;
                this.PromoFooter = "Entertainment news coming up next >>";
                break;
            case CeefaxSectionType.Entertainment:
                this.Header = Graphics.HeaderEntertainment;
                break;
          
            default:
                break;
        }
    }
}
