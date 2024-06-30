using System.Diagnostics;
using API.Architecture;
using API.Magazine;
using API.PageGenerators;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace API.Services
{
    public class CarouselDelivery
    {
        public TeletextCarousel Carousel { get; set; }
        public int TotalCarousels { get; set; } = 0;
        public int TotalRequests { get; set; } = 0;
        public DateTime LastBuilt { get; set; }
        public int BuildTimeSeconds {get; set; }
    }

    public interface ICacheService
    {
        public CarouselDelivery GetCarousel();
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cacheCarousel = new MemoryCache(new MemoryCacheOptions());
        private readonly Object l = new();
        private readonly DateTime _serviceStart = Utility.ConvertToUKTime(DateTime.UtcNow);
        private CarouselDelivery _cd = new();
        private readonly ISystemConfig _config;

        public CacheService(ISystemConfig config)
        {
            _config = config;
        }

        public CarouselDelivery GetCarousel()
        {
            _cd.TotalRequests++;
            Log.Information($"New request received ({_cd.TotalRequests} total)");

            lock (l)
            {
                var content = _cacheCarousel.Get<TeletextCarousel>("carousel");
                if (content is null)
                {
                    try
                    {
                        _cd.TotalCarousels++;
                        Log.Information($"Building new carousel ({_cd.TotalCarousels} total)");

                        var sw = new Stopwatch();
                        sw.Start();

                        // Instantiate objects required to build cache
                        CeefaxContent cc = new(_config);
                        IMarketData marketData = new MarketData(cc);
                        IStandingData standingData = new StandingsData(cc);
                        IWeatherData weatherData = new WeatherData(cc);
                        ITVListingData listingData = new TVListingData(cc);
                        
                        CarouselService cs = new(
                            new TeletextPageNews(cc),
                            new TeletextPageMarkets(cc, marketData),
                            new TeletextPageStandings(cc, standingData),
                            new TeletextPageWeather(cc, weatherData),
                            new TeletextPageTV(cc, listingData));
                        
                        _cd.Carousel = cs.Carousel;
                        _cd.Carousel.IsValid = marketData.IsValid && standingData.IsValid && weatherData.IsValid && listingData.IsValid;
                        _cd.LastBuilt = Utility.ConvertToUKTime(DateTime.UtcNow);
                        _cd.BuildTimeSeconds = Convert.ToInt32(sw.ElapsedMilliseconds / 1000);

                        _cacheCarousel.Set("carousel", _cd.Carousel, TimeSpan.FromMinutes(_config.ServiceContentExpiryMins));
                    }
                    catch(Exception ex) 
                    {
                        Log.Fatal($"CAROUSEL BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}");
                        Log.CloseAndFlush();
                    }
                }

                return _cd;
            }
        }
    }
}
