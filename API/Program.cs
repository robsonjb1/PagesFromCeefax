using API.Architecture;
using API.Extensions;
using API.Magazine;
using API.PageGenerators;
using API.Services;
using Microsoft.Extensions.FileProviders;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
 
try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddSingleton<ISystemConfig>(new SystemConfig()
    {
        OpenWeatherApiKey = builder.Configuration["OpenWeatherApiKey"],
        ServiceContentExpiryMins = Convert.ToInt32(builder.Configuration["ServiceContentExpiryMins"])
    });
    builder.Services.AddSingleton<ICacheService, CacheService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseHttpsRedirection();

    app.MapGet("/carousel", (ICacheService cs) =>
    {
        return Results.Extensions.NoCache(cs.GetMagazine());
    });

    app.UseFileServer(new FileServerOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
        RequestPath = "",
        EnableDefaultFiles = true
    });

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException" &&
                           ex.GetType().Name is not "HostAbortedException")
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
