using API;
using API.AppCode.Magazine;
using Microsoft.Extensions.FileProviders;
using PagesFromCeefax;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<MagazineContent>();
builder.Services.AddSingleton<ITeletextPageNews, TeletextPageNews>();
builder.Services.AddSingleton<ITeletextPageWeather, TeletextPageWeather>();

builder.Services.AddSingleton<ICarousel, Carousel>();
builder.Services.AddSingleton<ICarouselCache, CarouselCache>();

builder.Services.AddSingleton<ISystemConfig>(new SystemConfig()
{
    OpenWeatherApiKey = builder.Configuration["OpenWeatherApiKey"]
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapGet("/carousel", (ICarouselCache cache) =>
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
