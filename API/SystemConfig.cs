using System;
namespace API
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
