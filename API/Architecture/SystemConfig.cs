using System;

namespace API.Architecture
{
    public interface ISystemConfig
    {
        public string OpenWeatherApiKey { get; set; }
    }

    public class SystemConfig : ISystemConfig
    {
        public string OpenWeatherApiKey { get; set; }
    }
}
