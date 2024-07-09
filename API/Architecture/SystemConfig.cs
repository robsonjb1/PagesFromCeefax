namespace API.Architecture
{
    public interface ISystemConfig
    {
        public string OpenWeatherApiKey { get; set; }
        public int ServiceContentExpiryMins { get; set; }
        public string KindleFromAddress { get; set; }
        public string KindleFromUsername { get; set; }
        public string KindleFromPassword { get; set; }
        public string KindleToAddress { get; set; }   
        public string KindleName { get; set; }
        public string KindleHost { get; set; }    
        public int KindlePort { get; set; }
        public bool KindleEnableSsl { get; set; }   
        public string SpecSessionCookie { get; set; }
        public string ThurrottSessionCookie { get; set; }
        public string StreamingCode { get; set; }
    }

    public class SystemConfig : ISystemConfig
    {
        public static List<string> WeatherCities {
            get { return new List<string> { "London", "Belfast", "Cardiff", "Edinburgh", "Lerwick", "Manchester", "Truro",
                "Paris", "Madrid", "Munich", "Krakow", "Cape Town", "Chennai", "Singapore", "Tokyo", "Sydney", "Wellington",
                "San Francisco", "New York" };}
        }
        public string OpenWeatherApiKey { get; set; }
        public int ServiceContentExpiryMins { get; set; }
        public string KindleFromAddress { get; set; }
        public string KindleFromUsername { get; set; }
        public string KindleFromPassword { get; set; }
        public string KindleToAddress { get; set; }   
        public string KindleName { get; set; }
        public string KindleHost { get; set; }    
        public int KindlePort { get; set; }
        public bool KindleEnableSsl { get; set; }   
        public string SpecSessionCookie { get; set; }
        public string ThurrottSessionCookie { get; set; }
        public string StreamingCode { get; set; }
    }
}
