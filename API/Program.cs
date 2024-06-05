using API.Architecture;
using API.Extensions;
using API.Services;
using Microsoft.Extensions.FileProviders;

Console.WriteLine("Starting PFC service");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ISystemConfig>(new SystemConfig()
{
    OpenWeatherApiKey = builder.Configuration["OpenWeatherApiKey"],
    ServiceContentExpiryMins = Convert.ToInt32(builder.Configuration["ServiceContentExpiryMins"]),
    SpecFromAddress = builder.Configuration["SpecFromAddress"],
    SpecFromUsername = builder.Configuration["SpecFromUsername"],
    SpecFromPassword = builder.Configuration["SpecFromPassword"],
    SpecToAddress = builder.Configuration["SpecToAddress"],
    SpecName = builder.Configuration["SpecName"],
    SpecHost = builder.Configuration["SpecHost"],
    SpecPort = Convert.ToInt32(builder.Configuration["SpecPort"]),
    SpecEnableSsl = Convert.ToBoolean(builder.Configuration["SpecEnableSsl"])
});
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IKindleService, KindleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapGet("/carousel", (ICacheService cs) =>
{
    return Results.Extensions.NoCache(cs.GetMagazine());
});

app.MapGet("/kindle/{email}", (IKindleService ks, string email) =>
{
    return ks.Publish(email);
});

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "",
    EnableDefaultFiles = true
});

app.Run();
