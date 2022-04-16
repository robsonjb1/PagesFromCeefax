using Microsoft.Extensions.FileProviders;
using PagesFromCeefax;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(new CarouselCache());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapGet("/carousel", (ILogger<Program> logger, CarouselCache cache) =>
{
    return Results.Extensions.NoCache(cache.GetMagazine(logger)) ;
});

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "",
    EnableDefaultFiles = true
});

app.Logger.LogInformation("Starting carousel service");
app.Run();
