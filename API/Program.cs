using API.Architecture;
using API.Extensions;
using API.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<ISystemConfig>(new SystemConfig()
{
    OpenWeatherApiKey = Environment.GetEnvironmentVariable("OpenWeatherApiKey") != null ?
        Environment.GetEnvironmentVariable("OpenWeatherApiKey") : builder.Configuration["OpenWeatherApiKey"],
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
