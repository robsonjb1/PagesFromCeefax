using System;
using System.Text;
using System.Web;

namespace PagesFromCeefax
{
    public static class MagazineCache
    {
        private static StringBuilder _currentMagazine = new StringBuilder();
        private static DateTime _timeStamp = DateTime.Now.AddYears(-1);
        private static Object l = new Object();
        private static int _totalRequests = 0;
        private static int _totalCarousels = 0;
        private static DateTime _serviceStart = DateTime.Now;

        public static StringBuilder CurrentMagazine
        {
            // Only refresh the magazine on the first get (not on service start)
            get
            {
                _totalRequests++;
                lock (l)
                {
                    if (DateTime.Now > _timeStamp.AddMinutes(20))
                    {
                        try
                        {
                            Carousel c = new Carousel();
                       
                            _currentMagazine = c.Content;
                            _totalCarousels++;
                        }
                        finally
                        {
                            _timeStamp = DateTime.Now;
                        }
                    }
                }

                return _currentMagazine
                    .Replace("{PFC_TOTALREQUESTS}", _totalRequests.ToString())
                    .Replace("{PFC_TOTALCAROUSELS}", _totalCarousels.ToString())
                    .Replace("{PFC_SERVICESTART}", _serviceStart.DayOfWeek.ToString().Substring(0, 3) + _serviceStart.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_TIMESTAMP}", _timeStamp.DayOfWeek.ToString().Substring(0, 3) + _timeStamp.ToString(" dd MMM HH:mm/ss"));
            }
        }
    }
}