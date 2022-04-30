using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace PagesFromCeefax
{
    public class CarouselCache : ICarouselCache
    {
        private readonly IMemoryCache _currentCarousel = new MemoryCache(new MemoryCacheOptions());

        private Object l = new Object();
        private int _totalRequests = 0;
        private int _totalCarousels = 0;
        private DateTime _serviceStart = DateTime.Now;
        private DateTime _lastBuilt = DateTime.Now;

        public string GetMagazine()
        {
            // Only refresh the magazine on the first get (not on service start)
            _totalRequests++;

            lock (l)
            {
                var content = _currentCarousel.Get<string>("carousel");
                if (content is null)
                {
                    _totalCarousels++;

                    var sw = new Stopwatch();
                    sw.Start();
                    var c = new CarouselBuilder();
                    _lastBuilt = DateTime.Now;

                    content = c.Content.DisplayHtml.ToString();
                    _currentCarousel.Set("carousel", content, TimeSpan.FromMinutes(20));
                }

                return content!
                    .Replace("{PFC_TOTALCAROUSELS}", _totalCarousels.ToString())
                    .Replace("{PFC_TOTALREQUESTS}", _totalRequests.ToString())
                    .Replace("{PFC_SERVICESTART}", _serviceStart.DayOfWeek.ToString().Substring(0, 3) + _serviceStart.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_TIMESTAMP}", _lastBuilt.DayOfWeek.ToString().Substring(0, 3) + _lastBuilt.ToString(" dd MMM HH:mm/ss"));
            }
        }

    }
}
