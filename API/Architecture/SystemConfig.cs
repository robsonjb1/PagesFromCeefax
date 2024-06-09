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
    }

    public class SystemConfig : ISystemConfig
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
    }
}
