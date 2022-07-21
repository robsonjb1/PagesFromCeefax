namespace API.Magazine
{ 
    public class WeatherData
    {
        public string TodayTitle { get; set; }
        public string TodayText { get; set; }
        public string TomorrowTitle { get; set; }
        public string TomorrowText { get; set; }
        public string OutlookTitle { get; set; }
        public string OutlookText { get; set; }

        public Dictionary<string, int> Temperatures = new();

        public DateTime LastRefreshUTC { get; set; }
    }
}