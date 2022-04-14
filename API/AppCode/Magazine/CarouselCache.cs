using System;
using System.Text;
using System.Web;
using Microsoft.Extensions.Caching.Memory;

namespace PagesFromCeefax
{
    public static class CarouselCache
    {
        private static readonly IMemoryCache _currentCarousel = new MemoryCache(new MemoryCacheOptions());

        private static Object l = new Object();
        private static int _totalRequests = 0;
        private static int _totalCarousels = 0;
        private static DateTime _serviceStart = DateTime.Now;
        private static DateTime _lastBuilt = DateTime.Now;
        
        public static string Current
        {
            // Only refresh the magazine on the first get (not on service start)
            get
            {
                _totalRequests++;
                lock (l)
                {
                    var content = _currentCarousel.Get<string>("carousel");
                    if (content is null)
                    {
                        _totalCarousels++;

                        var c = new CarouselBuilder();
                        content = c.Content.DisplayHtml.ToString();
                        _currentCarousel.Set("carousel", content, TimeSpan.FromMinutes(30));
                        _lastBuilt = DateTime.Now;
                    }

                    return content!
                        .Replace("{PFC_TOTALREQUESTS}", _totalRequests.ToString())
                        .Replace("{PFC_TOTALCAROUSELS}", _totalCarousels.ToString())
                        .Replace("{PFC_SERVICESTART}", _serviceStart.DayOfWeek.ToString().Substring(0, 3) + _serviceStart.ToString(" dd MMM HH:mm/ss"))
                        .Replace("{PFC_TIMESTAMP}", _lastBuilt.DayOfWeek.ToString().Substring(0, 3) + _lastBuilt.ToString(" dd MMM HH:mm/ss"));
                }
            }
        }
    }
}