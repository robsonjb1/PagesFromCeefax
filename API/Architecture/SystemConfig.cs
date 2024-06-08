namespace API.Architecture
{
    public interface ISystemConfig
    {
        public string OpenWeatherApiKey { get; set; }
        public int ServiceContentExpiryMins { get; set; }
        public string SpecFromAddress { get; set; }
        public string SpecFromUsername { get; set; }
        public string SpecFromPassword { get; set; }
        public string SpecToAddress { get; set; }   
        public string SpecName { get; set; }
        public string SpecHost { get; set; }    
        public int SpecPort { get; set; }
        public bool SpecEnableSsl { get; set; }   
        public string SpecSessionCookie { get; set; }
    }

    public class SystemConfig : ISystemConfig
    {
        public string OpenWeatherApiKey { get; set; }
        public int ServiceContentExpiryMins { get; set; }
        public string SpecFromAddress { get; set; }
        public string SpecFromUsername { get; set; }
        public string SpecFromPassword { get; set; }
        public string SpecToAddress { get; set; }   
        public string SpecName { get; set; }
        public string SpecHost { get; set; }    
        public int SpecPort { get; set; }
        public bool SpecEnableSsl { get; set; }   
        public string SpecSessionCookie { get; set; }
    }
}
