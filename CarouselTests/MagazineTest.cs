using System.IO;
using API.Architecture;
using API.Magazine;
using API.Services;
using Microsoft.Extensions.Configuration;

namespace CarouselTests;

[TestClass]
public class MagazineTest
{
    private static MagazineContent _mc;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(@"appsettings.json", false, false)
            .AddEnvironmentVariables()
            .Build();

        _mc = new MagazineContent(new SystemConfig()
        {
            OpenWeatherApiKey = configuration["OpenWeatherApiKey"],
            ServiceContentExpiryMins = Convert.ToInt32(configuration["ServiceContentExpiryMins"])
        });
    }

    [TestMethod]
    public void RetrieveContent()
    {
        // Ensure we can retrieve data from all the RSS URL's and parse the story data
        Assert.IsTrue(_mc.StoryList.Count > 0);
    }

    [TestMethod]
    public void ParseWeatherData()
    {
        WeatherService ws = new WeatherService(_mc);
        WeatherData wd = ws.GetWeatherData();

        // Check all weather locations can be retrieved
        Assert.IsTrue(wd.Temperatures.Count == 7);
    }

    [TestMethod]
    public void ParseMarketsData()
    {
        MarketService ms = new MarketService(_mc);
        MarketData md = ms.GetMarketData();

        Assert.IsTrue(md.Markets.Count > 0);
    }
}

