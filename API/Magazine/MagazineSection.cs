﻿using System.Text;
using API.Architecture;

namespace API.Magazine
{
    public class MagazineSection
    {
        public readonly MagazineSectionType Name;
        public readonly Uri Feed;
        public readonly int TotalStories;
        public readonly bool HasNewsInBrief = false;
        public readonly StringBuilder Header;
        public readonly Mode7Colour HeadingCol;
        public readonly string PromoFooter;
        public readonly Mode7Colour PromoPaper;
        public readonly Mode7Colour PromoInk;

        public MagazineSection(MagazineSectionType Name, Uri Feed)
        {
            this.Name = Name;
            this.Feed = Feed;
           
            // Initialise the visual parameters depending on section
            switch (Name)
            {
                case MagazineSectionType.Home:
                case MagazineSectionType.World:
                case MagazineSectionType.Politics:
                case MagazineSectionType.Science:
                case MagazineSectionType.Technology:
                case MagazineSectionType.Sussex:
                    this.HeadingCol = Mode7Colour.Yellow;
                    this.PromoPaper = Mode7Colour.Blue;
                    this.PromoInk = Mode7Colour.Yellow;
                    break;

                case MagazineSectionType.Business:
                    this.HeadingCol = Mode7Colour.Yellow;
                    this.PromoPaper = Mode7Colour.Red;
                    this.PromoInk = Mode7Colour.White;
                    break;

                case MagazineSectionType.Football:
                case MagazineSectionType.Rugby:
                case MagazineSectionType.Cricket:
                case MagazineSectionType.Tennis:
                case MagazineSectionType.Golf:
                case MagazineSectionType.Formula1:
                    this.HeadingCol = Mode7Colour.Green;
                    this.PromoPaper = Mode7Colour.Blue;
                    this.PromoInk = Mode7Colour.Yellow;
                    break;

                case MagazineSectionType.Entertainment:
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
                case MagazineSectionType.Home:
                case MagazineSectionType.World:
                case MagazineSectionType.Politics:
                case MagazineSectionType.Business:
                case MagazineSectionType.Football:
                case MagazineSectionType.Entertainment:
                    this.TotalStories = 3;
                    this.HasNewsInBrief = true;
                    break;

                case MagazineSectionType.Markets:
                case MagazineSectionType.WeatherForecast:
                case MagazineSectionType.WeatherTempBelfast:
                case MagazineSectionType.WeatherTempCardiff:
                case MagazineSectionType.WeatherTempEdinburgh:
                case MagazineSectionType.WeatherTempLerwick:
                case MagazineSectionType.WeatherTempLondon:
                case MagazineSectionType.WeatherTempManchester:
                case MagazineSectionType.WeatherTempTruro:
                    this.TotalStories = 0;
                    break;

                default:
                    this.TotalStories = 2;
                    break;
            }


            // Initialise header banner and promo footer text
            switch (Name)
            {
                case MagazineSectionType.Home:
                    this.Header = Graphics.HeaderHome;
                    this.PromoFooter = "World news coming up next >>>";
                    break;
                case MagazineSectionType.World:
                    this.Header = Graphics.HeaderWorld;
                    this.PromoFooter = "Political news coming up next >>>";
                    break;
                case MagazineSectionType.Politics:
                    this.Header = Graphics.HeaderPolitics;
                    this.PromoFooter = "Technology news coming up next >>>";
                    break;
                case MagazineSectionType.Science:
                    this.Header = Graphics.HeaderSciTech;
                    break;
                case MagazineSectionType.Technology:
                    this.Header = Graphics.HeaderSciTech;
                    this.PromoFooter = "Sussex news coming up next >>>";
                    break;
                case MagazineSectionType.Sussex:
                    this.Header = Graphics.HeaderSussex;
                    this.PromoFooter = "Business news coming up next >>>";
                    break;
                case MagazineSectionType.Business:
                    this.Header = Graphics.HeaderBusiness;
                    break;

                case MagazineSectionType.Football:
                    this.Header = Graphics.HeaderFootball;
                    this.PromoFooter = "Rugby news coming up next >>>";
                    break;
                case MagazineSectionType.Rugby:
                    this.Header = Graphics.HeaderRugby;
                    this.PromoFooter = "Cricket news coming up next >>>";
                    break;
                case MagazineSectionType.Cricket:
                    this.Header = Graphics.HeaderCricket;
                    this.PromoFooter = "Tennis news coming up next >>>";
                    break;
                case MagazineSectionType.Tennis:
                    this.Header = Graphics.HeaderTennis;
                    this.PromoFooter = "Golf news coming up next >>>";
                    break;
                case MagazineSectionType.Golf:
                    this.Header = Graphics.HeaderGolf;
                    this.PromoFooter = "Motorsport news coming up next >>>";
                    break;
                case MagazineSectionType.Formula1:
                    this.Header = Graphics.HeaderFormula1;
                    break;

                case MagazineSectionType.Entertainment:
                    this.Header = Graphics.HeaderEntertainment;
                    break;


                default:
                    break;
            }
        }
    }
}
