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

        private Object l = new Object();
        private int _totalCarousels = 0;
        private readonly DateTime _serviceStart = DateTime.Now;
        private DateTime _lastBuilt = DateTime.Now;
        private long _buildTime = 0;
        private readonly Dictionary<DateOnly, int> _totalRequests = new();

        private readonly ICarouselService _carousel;
        private readonly ISystemConfig _config;

        public CacheService(ICarouselService carousel, ISystemConfig config)
        {
            _carousel = carousel;
            _config = config;
        }

        public string GetMagazine()
        {
            // Log request count per day
            DateOnly todaysDate = DateOnly.FromDateTime(DateTime.Now);
            if (_totalRequests.ContainsKey(todaysDate))
            {
                _totalRequests[todaysDate]++;
            }
            else
            {
                _totalRequests.Add(todaysDate, 1);
            }

            lock (l)
            {
                var content = _currentCarousel.Get<string>("carousel");
                if (content is null)
                {
                    _totalCarousels++;

                    var sw = new Stopwatch();
                    sw.Start();
                    content = _carousel.GetCarousel();
                    _lastBuilt = DateTime.Now;
                    _buildTime = sw.ElapsedMilliseconds;

                    _currentCarousel.Set("carousel", content, TimeSpan.FromMinutes(_config.ServiceContentExpiryMins));
                }

                var requestLog = new StringBuilder();
                foreach(var key in _totalRequests.Keys)
                {
                    requestLog.AppendLine(String.Format("<!-- {0}: {1} -->",
                        key.DayOfWeek.ToString()[..3] + key.ToString(" dd MMM"),
                        _totalRequests[key]));
                }

                return content
                    .Replace("{PFC_TOTALCAROUSELS}", _totalCarousels.ToString())
                    .Replace("{PFC_TOTALREQUESTS}", requestLog.ToString())
                    .Replace("{PFC_SERVICESTART}", _serviceStart.DayOfWeek.ToString()[..3] + _serviceStart.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_TIMESTAMP}", _lastBuilt.DayOfWeek.ToString()[..3] + _lastBuilt.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_BUILDTIME}", _buildTime.ToString());
            }
        }

    }
}
