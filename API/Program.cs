using API.Architecture;
using API.Extensions;
using API.Magazine;
using API.PageGenerators;
using API.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IMagazineContent, MagazineContent>();
builder.Services.AddSingleton<IWeatherService, WeatherService>();
builder.Services.AddSingleton<ITeletextPageNews, TeletextPageNews>();
builder.Services.AddSingleton<ITeletextPageWeather, TeletextPageWeather>();
builder.Services.AddSingleton<ICarouselService, CarouselService>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<ISystemConfig>(new SystemConfig()
{
    OpenWeatherApiKey = builder.Configuration["OpenWeatherApiKey"],
    ServiceContentExpiryMins = Convert.ToInt32(builder.Configuration["ServiceContentExpiryMins"])
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapGet("/carousel", (ICacheService cache) =>
{
    return Results.Extensions.NoCache(cache.GetMagazine());
});

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "",
    EnableDefaultFiles = true
});

app.Run();
