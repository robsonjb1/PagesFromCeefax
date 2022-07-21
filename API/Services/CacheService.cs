using System.Diagnostics;
using System.Text;
using API.Architecture;
using API.Magazine;
using API.PageGenerators;
using Microsoft.Extensions.Caching.Memory;

namespace API.Services
{
    public interface ICacheService
    {
        public string GetMagazine();
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _currentCarousel = new MemoryCache(new MemoryCacheOptions());

        private readonly Object l = new Object();
        private int _totalCarousels = 0;
        private int _totalRequests = 0;
        private readonly DateTime _serviceStart = DateTime.Now;
        private DateTime _lastBuilt = DateTime.Now;
        private long _buildTime = 0;

        private readonly ISystemConfig _config;

        public CacheService(ISystemConfig config)
        {
            _config = config;
        }

        public string GetMagazine()
        {
            _totalRequests++;

            lock (l)
            {
                var content = _currentCarousel.Get<string>("carousel");
                if (content is null)
                {
                    _totalCarousels++;

                    var sw = new Stopwatch();
                    sw.Start();

                    // Instantiate objects required to build cache
                    // These were previously in the DI container but moved due to issues with caching
                    MagazineContent mc = new(_config);
                    WeatherService ws = new(mc);
                    TeletextPageWeather tw = new(ws);
                    TeletextPageNews tn = new(mc);
                    CarouselService cs = new(tn, tw);

                    content = cs.GetCarousel();

                    _lastBuilt = DateTime.Now;
                    _buildTime = sw.ElapsedMilliseconds;

                    _currentCarousel.Set("carousel", content, TimeSpan.FromMinutes(_config.ServiceContentExpiryMins));
                }

                return content
                    .Replace("{PFC_TOTALCAROUSELS}", _totalCarousels.ToString())
                    .Replace("{PFC_TOTALREQUESTS}", _totalRequests.ToString())
                    .Replace("{PFC_SERVICESTART}", _serviceStart.DayOfWeek.ToString()[..3] + _serviceStart.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_TIMESTAMP}", _lastBuilt.DayOfWeek.ToString()[..3] + _lastBuilt.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_BUILDTIME}", _buildTime.ToString());
            }
        }

    }
}
