using System.Text;
using API.Architecture;

namespace API.Magazine;

public class CeefaxSection
{
    public readonly CeefaxSectionType Name;
    public readonly Uri Feed;
    public readonly int TotalPages;
    public readonly StringBuilder Header;
    public readonly TeletextControl HeadingCol;
    public readonly string PromoFooter;
    public readonly TeletextControl PromoBackground;
    public readonly TeletextControl PromoCol;

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
            case CeefaxSectionType.Kent:
            case CeefaxSectionType.TVScheduleBBC1:
            case CeefaxSectionType.TVScheduleBBC2:
            case CeefaxSectionType.TVScheduleBBC4:
            case CeefaxSectionType.Weather:
            case CeefaxSectionType.WeatherForecast1:
            case CeefaxSectionType.WeatherForecast2:
            case CeefaxSectionType.WeatherForecast3:
            case CeefaxSectionType.Travel:
                HeadingCol = TeletextControl.AlphaYellow;
                PromoBackground = TeletextControl.AlphaBlue;
                PromoCol = TeletextControl.AlphaYellow;
                break;

            case CeefaxSectionType.Business:
            case CeefaxSectionType.Markets:
            case CeefaxSectionType.TheRegister:
                HeadingCol = TeletextControl.AlphaYellow;
                PromoBackground = TeletextControl.AlphaRed;
                PromoCol = TeletextControl.AlphaWhite;
                break;

            case CeefaxSectionType.Football:
            case CeefaxSectionType.Rugby:
            case CeefaxSectionType.Cricket:
            case CeefaxSectionType.Tennis:
            case CeefaxSectionType.Golf:
            case CeefaxSectionType.Motorsport:
            case CeefaxSectionType.Standings:
                HeadingCol = TeletextControl.AlphaGreen;
                PromoBackground = TeletextControl.AlphaBlue;
                PromoCol = TeletextControl.AlphaYellow;
                break;

            case CeefaxSectionType.Entertainment:
                HeadingCol = TeletextControl.AlphaYellow;
                PromoBackground = TeletextControl.AlphaMagenta;
                PromoCol = TeletextControl.AlphaYellow;
                break;

            default:
                break;
        }

        // Initialise the full section sizes
        switch (Name)
        {
            case CeefaxSectionType.Standings:
            case CeefaxSectionType.Weather:
            case CeefaxSectionType.WeatherForecast1:
            case CeefaxSectionType.WeatherForecast2:
            case CeefaxSectionType.WeatherForecast3:
            case CeefaxSectionType.WeatherLondon:
            case CeefaxSectionType.WeatherManchester:
            case CeefaxSectionType.WeatherTruro:
            case CeefaxSectionType.WeatherBelfast:
            case CeefaxSectionType.WeatherCardiff:
            case CeefaxSectionType.WeatherEdinburgh:
            case CeefaxSectionType.WeatherLerwick:
            case CeefaxSectionType.TVScheduleBBC1:
            case CeefaxSectionType.TVScheduleBBC2:
            case CeefaxSectionType.TVScheduleBBC4:
            case CeefaxSectionType.Markets:
            case CeefaxSectionType.MarketsCurrencies:
            case CeefaxSectionType.MarketsBonds:
            case CeefaxSectionType.Travel:
                TotalPages = 0;
                break;
            
            case CeefaxSectionType.Home:
            case CeefaxSectionType.World:
            case CeefaxSectionType.Politics:
            case CeefaxSectionType.Business:
            case CeefaxSectionType.Football:
            case CeefaxSectionType.Entertainment:
                TotalPages = 2;
                break;

            case CeefaxSectionType.TheRegister:
                TotalPages = 3;
                break;

            default:
                TotalPages = 1;
                break;
        }

        // Initialise header banner and promo footer text
        switch (Name)
        {
            case CeefaxSectionType.Home:
                Header = Graphics.HeaderHome;
                PromoFooter = "World news coming up next >>>";
                break;
            case CeefaxSectionType.World:
                Header = Graphics.HeaderWorld;
                PromoFooter = "Political news coming up next >>>";
                break;
            case CeefaxSectionType.Politics:
                Header = Graphics.HeaderPolitics;
                PromoFooter = "Science news coming up next >>>";
                break;
            case CeefaxSectionType.Science:
                Header = Graphics.HeaderSciTech;
                PromoFooter = "Technology news coming up next >>>";
                break;
            case CeefaxSectionType.Technology:
                Header = Graphics.HeaderSciTech;
                PromoFooter = "Kent news coming up next >>>";
                break;
            case CeefaxSectionType.Sussex:
                Header = Graphics.HeaderSussex;
                PromoFooter = "Travel news coming up next >>>";
                break;
            case CeefaxSectionType.Kent:
                Header = Graphics.HeaderKent;
                PromoFooter = "Sussex news coming up next >>>";
                break;
            case CeefaxSectionType.Business:
                Header = Graphics.HeaderBusiness;
                PromoFooter = "Market data coming up next >>>";    
                break;
            case CeefaxSectionType.Football:
                Header = Graphics.HeaderFootball;
                PromoFooter = "Rugby news coming up next >>>";
                break;
            case CeefaxSectionType.Rugby:
                Header = Graphics.HeaderRugby;
                PromoFooter = "Cricket news coming up next >>>";
                break;
            case CeefaxSectionType.Cricket:
                Header = Graphics.HeaderCricket;
                PromoFooter = "Tennis news coming up next >>>";
                break;
            case CeefaxSectionType.Tennis:
                Header = Graphics.HeaderTennis;
                PromoFooter = "Golf news coming up next >>>";
                break;
            case CeefaxSectionType.Golf:
                Header = Graphics.HeaderGolf;
                PromoFooter = "Motorsport news coming up next >>>";
                break;
            case CeefaxSectionType.Motorsport:
                Header = Graphics.HeaderFormula1;
                break;
            case CeefaxSectionType.TVScheduleBBC4:
                Header = Graphics.HeaderGolf;
                PromoFooter = "Entertainment news coming up next >>";
                break;
            case CeefaxSectionType.Entertainment:
                Header = Graphics.HeaderEntertainment;
                PromoFooter = "The Register coming up next >>";
                break;
            case CeefaxSectionType.Weather:
                Header = Graphics.HeaderWeather;
                break;
            case CeefaxSectionType.TheRegister:
                Header = Graphics.HeaderRegister;
                PromoFooter = "CEEFAX: The world at your fingertips";
                break;
            default:
                break;
        }
    }
}
