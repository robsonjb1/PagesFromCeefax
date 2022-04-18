using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace PagesFromCeefax
{
    public class CarouselCache
    {
        private string _currentCarousel = String.Empty;
        private DateTime _lastBuilt = DateTime.Now.AddYears(-1);
        private Object l = new Object();
        private int _totalRequests = 0;
        private int _totalCarousels = 0;
        private DateTime _serviceStart = DateTime.Now;
    
        public string GetMagazine(ILogger logger)
        {
            // Only refresh the magazine on the first get (not on service start)
            _totalRequests++;
            lock (l)
            {
                if (DateTime.Now > _lastBuilt.AddMinutes(20))
                { 
                    _totalCarousels++;

                    DateTime start = DateTime.Now;
                    CarouselBuilder cb = new CarouselBuilder();
                    _currentCarousel = cb.Content.DisplayHtml.ToString();
                    DateTime end = _lastBuilt = DateTime.Now;

                    logger.LogInformation($"Generated new carousel #{_totalCarousels} in {(end - start).TotalMilliseconds}ms");
                }

                logger.LogInformation($"Returning carousel request {_totalRequests}");

                return _currentCarousel
                    .Replace("{PFC_TOTALREQUESTS}", _totalRequests.ToString())
                    .Replace("{PFC_TOTALCAROUSELS}", _totalCarousels.ToString())
                    .Replace("{PFC_SERVICESTART}", _serviceStart.DayOfWeek.ToString().Substring(0, 3) + _serviceStart.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_TIMESTAMP}", _lastBuilt.DayOfWeek.ToString().Substring(0, 3) + _lastBuilt.ToString(" dd MMM HH:mm/ss"));
            }
        }
    }
}

