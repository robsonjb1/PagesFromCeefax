using System.Diagnostics;
using API.Architecture;
using API.Magazine;
using API.PageGenerators;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace API.Services
{
    public interface ICacheService
    {
        public string GetMagazine();
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _currentCarousel = new MemoryCache(new MemoryCacheOptions());
        private readonly Object l = new();
        private int _totalCarousels = 0;
        private int _totalRequests = 0;
        private readonly DateTime _serviceStart = Utility.ConvertToUKTime(DateTime.UtcNow);
        private DateTime _lastBuilt = Utility.ConvertToUKTime(DateTime.UtcNow);
        private long _buildTime = 0;

        private readonly ISystemConfig _config;

        public CacheService(ISystemConfig config)
        {
            _config = config;
        }

        public string GetMagazine()
        {
            _totalRequests++;
            Log.Information($"New request received ({_totalRequests} total)");

            lock (l)
            {
                var content = _currentCarousel.Get<string>("carousel");
                if (content is null)
                {
                    try
                    {
                        _totalCarousels++;
                        Log.Information($"Building new carousel ({_totalCarousels} total)");

                        var sw = new Stopwatch();
                        sw.Start();

                        // Instantiate objects required to build cache
                        CeefaxContent cc = new(_config);
                        IMarketData marketData = new MarketData(cc);
                        IWeatherData weatherData = new WeatherData(cc);
                        ITVListingData listingData = new TVListingData(cc);

                        CarouselService cs = new(
                            new TeletextPageNews(cc),
                            new TeletextPageWeather(cc, weatherData),
                            new TeletextPageMarkets(cc, marketData),
                            new TeletextPageTV(cc, listingData));
                            
                        content = cs.GetCarousel(marketData.IsValid && weatherData.IsValid && listingData.IsValid);
                    
                        _lastBuilt = Utility.ConvertToUKTime(DateTime.UtcNow);
                        _buildTime = sw.ElapsedMilliseconds;

                        _currentCarousel.Set("carousel", content, TimeSpan.FromMinutes(_config.ServiceContentExpiryMins));
                    }
                    catch(Exception ex) 
                    {
                        Log.Fatal($"CAROUSEL BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
                        Log.CloseAndFlush();
                    }
                }

                return content
                    .Replace("{PFC_TOTALCAROUSELS}", _totalCarousels.ToString())
                    .Replace("{PFC_TOTALREQUESTS}", _totalRequests.ToString())
                    .Replace("{PFC_SERVICESTART}", _serviceStart.DayOfWeek.ToString()[..3] + _serviceStart.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_TIMESTAMP}", _lastBuilt.DayOfWeek.ToString()[..3] + _lastBuilt.ToString(" dd MMM HH:mm/ss"))
                    .Replace("{PFC_BUILDTIME}", _buildTime.ToString("#,#0"));
            }
        }
    }
}
