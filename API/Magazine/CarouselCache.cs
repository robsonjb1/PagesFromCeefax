using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace API.Magazine
{
    public interface ICarouselCache
    {
        public string GetMagazine();
    }

    public class CarouselCache : ICarouselCache
    {
        private readonly IMemoryCache _currentCarousel = new MemoryCache(new MemoryCacheOptions());

        private Object l = new Object();
        private int _totalCarousels = 0;
        private DateTime _serviceStart = DateTime.Now;
        private DateTime _lastBuilt = DateTime.Now;
        private long _buildTime = 0;
        private Dictionary<DateOnly, int> _totalRequests = new();

        private ICarousel _carousel;

        public CarouselCache(ICarousel carousel)
        {
            _carousel = carousel;
        }

        public string GetMagazine()
        {
            // Only refresh the magazine on the first get (not on service start)

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
                    content = _carousel.BuildCarousel();
                    _lastBuilt = DateTime.Now;
                    _buildTime = sw.ElapsedMilliseconds;

                    _currentCarousel.Set("carousel", content, TimeSpan.FromMinutes(20));
                }

                var requestLog = new StringBuilder();
                foreach(var key in _totalRequests.Keys)
                {
                    requestLog.AppendLine(String.Format("<!-- {0}: {1} -->",
                        key.DayOfWeek.ToString().Substring(0, 3) + key.ToString(" dd MMM"),
                        _totalRequests[key]));
                }

                return content!
                    .Replace("{PFC_TOTALCAROUSELS}", _totalCarousels.ToString())
                    .Replace("{PFC_TOTALREQUESTS}", requestLog.ToString())
                    .Replace("{PFC_SERVICESTART}", _serviceStart.DayOfWeek.ToString().Substring(0, 3) + _serviceStart.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_TIMESTAMP}", _lastBuilt.DayOfWeek.ToString().Substring(0, 3) + _lastBuilt.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_BUILDTIME}", _buildTime.ToString());
            }
        }

    }
}
