using Microsoft.Extensions.Caching.Memory;

namespace PagesFromCeefax
{
    public class CarouselCache
    {
        private readonly IMemoryCache _currentCarousel = new MemoryCache(new MemoryCacheOptions());

        private Object l = new Object();
        private int _totalRequests = 0;
        private int _totalCarousels = 0;
        private DateTime _serviceStart = DateTime.Now;
        private DateTime _lastBuilt = DateTime.Now;

        public string GetMagazine(ILogger logger)
        {
            // Only refresh the magazine on the first get (not on service start)
            _totalRequests++;
            lock (l)
            {
                var content = _currentCarousel.Get<string>("carousel");
                if (content is null)
                {
                    _totalCarousels++;

                    DateTime start = DateTime.Now;
                    var c = new CarouselBuilder();
                    DateTime end = DateTime.Now;

                    content = c.Content.DisplayHtml.ToString();
                    _currentCarousel.Set("carousel", content, TimeSpan.FromMinutes(30));
                    _lastBuilt = DateTime.Now;

                    logger.LogInformation($"Generated new carousel #{_totalCarousels} in {(end - start).TotalMilliseconds}ms");
                }

                logger.LogInformation($"Returning carousel request {_totalRequests}");

                return content!
                    .Replace("{PFC_TOTALREQUESTS}", _totalRequests.ToString())
                    .Replace("{PFC_TOTALCAROUSELS}", _totalCarousels.ToString())
                    .Replace("{PFC_SERVICESTART}", _serviceStart.DayOfWeek.ToString().Substring(0, 3) + _serviceStart.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_TIMESTAMP}", _lastBuilt.DayOfWeek.ToString().Substring(0, 3) + _lastBuilt.ToString(" dd MMM HH:mm/ss"));
            }
        }
    }
}