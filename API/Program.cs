using Microsoft.Extensions.FileProviders;
using PagesFromCeefax;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(new CarouselCache());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapGet("/carousel", (CarouselCache cache) =>
{
    return Results.Extensions.NoCache(cache.GetMagazine());
});

app.MapGet("/activity", (CarouselCache cache) =>
{
    return Results.Extensions.NoCache(cache.ShowActivity());
});


app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = "",
    EnableDefaultFiles = true
});

app.Run();
