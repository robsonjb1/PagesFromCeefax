using API.Architecture;
using API.Extensions;
using API.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.FileProviders;
using Serilog;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

Log.Information("Starting PFC service");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .ReadFrom.Configuration(context.Configuration));

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
