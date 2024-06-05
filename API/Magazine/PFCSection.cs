using System.Text;
using API.Architecture;

namespace API.Magazine;

public class PFCSection
{
    public readonly PFCSectionType Name;
    public readonly Uri Feed;
    public readonly int TotalStories;
    public readonly bool HasNewsInBrief = false;
    public readonly StringBuilder Header;
    public readonly Mode7Colour HeadingCol;
    public readonly string PromoFooter;
    public readonly Mode7Colour PromoPaper;
    public readonly Mode7Colour PromoInk;

    public PFCSection(PFCSectionType Name, Uri Feed)
    {
        this.Name = Name;
        this.Feed = Feed;
       
        // Initialise the visual parameters depending on section
        switch (Name)
        {
            case PFCSectionType.Home:
            case PFCSectionType.World:
            case PFCSectionType.Politics:
            case PFCSectionType.Science:
            case PFCSectionType.Technology:
            case PFCSectionType.Sussex:
            case PFCSectionType.TVScheduleBBC1:
            case PFCSectionType.TVScheduleBBC2:
            case PFCSectionType.TVScheduleBBC4:
            case PFCSectionType.WeatherForecast:
                this.HeadingCol = Mode7Colour.Yellow;
                this.PromoPaper = Mode7Colour.Blue;
                this.PromoInk = Mode7Colour.Yellow;
                break;

            case PFCSectionType.Business:
            case PFCSectionType.Markets:
                this.HeadingCol = Mode7Colour.Yellow;
                this.PromoPaper = Mode7Colour.Red;
                this.PromoInk = Mode7Colour.White;
                break;

            case PFCSectionType.Football:
            case PFCSectionType.Rugby:
            case PFCSectionType.Cricket:
            case PFCSectionType.Tennis:
            case PFCSectionType.Golf:
            case PFCSectionType.Formula1:
                this.HeadingCol = Mode7Colour.Green;
                this.PromoPaper = Mode7Colour.Blue;
                this.PromoInk = Mode7Colour.Yellow;
                break;

            case PFCSectionType.Entertainment:
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
            case PFCSectionType.Home:
            case PFCSectionType.World:
            case PFCSectionType.Politics:
            case PFCSectionType.Business:
            case PFCSectionType.Football:
            case PFCSectionType.Entertainment:
                this.TotalStories = 3;
                this.HasNewsInBrief = true;
                break;

            case PFCSectionType.Markets:
            case PFCSectionType.WeatherForecast:
            case PFCSectionType.WeatherTempBelfast:
            case PFCSectionType.WeatherTempCardiff:
            case PFCSectionType.WeatherTempEdinburgh:
            case PFCSectionType.WeatherTempLerwick:
            case PFCSectionType.WeatherTempLondon:
            case PFCSectionType.WeatherTempManchester:
            case PFCSectionType.WeatherTempTruro:
            case PFCSectionType.TVScheduleBBC1:
            case PFCSectionType.TVScheduleBBC2:
            case PFCSectionType.TVScheduleBBC4:
                this.TotalStories = 0;
                break;

            default:
                this.TotalStories = 2;
                break;
        }


        // Initialise header banner and promo footer text
        switch (Name)
        {
            case PFCSectionType.Home:
                this.Header = Graphics.HeaderHome;
                this.PromoFooter = "World news coming up next >>>";
                break;
            case PFCSectionType.World:
                this.Header = Graphics.HeaderWorld;
                this.PromoFooter = "Political news coming up next >>>";
                break;
            case PFCSectionType.Politics:
                this.Header = Graphics.HeaderPolitics;
                this.PromoFooter = "Technology news coming up next >>>";
                break;
            case PFCSectionType.Science:
                this.Header = Graphics.HeaderSciTech;
                break;
            case PFCSectionType.Technology:
                this.Header = Graphics.HeaderSciTech;
                this.PromoFooter = "Sussex news coming up next >>>";
                break;
            case PFCSectionType.Sussex:
                this.Header = Graphics.HeaderSussex;
                this.PromoFooter = "Business news coming up next >>>";
                break;
            case PFCSectionType.Business:
                this.Header = Graphics.HeaderBusiness;
                this.PromoFooter = "Market data coming up next >>>";
                break;
            case PFCSectionType.Football:
                this.Header = Graphics.HeaderFootball;
                this.PromoFooter = "Rugby news coming up next >>>";
                break;
            case PFCSectionType.Rugby:
                this.Header = Graphics.HeaderRugby;
                this.PromoFooter = "Cricket news coming up next >>>";
                break;
            case PFCSectionType.Cricket:
                this.Header = Graphics.HeaderCricket;
                this.PromoFooter = "Tennis news coming up next >>>";
                break;
            case PFCSectionType.Tennis:
                this.Header = Graphics.HeaderTennis;
                this.PromoFooter = "Golf news coming up next >>>";
                break;
            case PFCSectionType.Golf:
                this.Header = Graphics.HeaderGolf;
                this.PromoFooter = "Motorsport news coming up next >>>";
                break;
            case PFCSectionType.Formula1:
                this.Header = Graphics.HeaderFormula1;
                break;
            case PFCSectionType.Entertainment:
                this.Header = Graphics.HeaderEntertainment;
                break;
          
            default:
                break;
        }
    }
}
