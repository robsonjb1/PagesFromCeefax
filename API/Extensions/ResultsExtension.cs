using API.Services;

namespace API.Extensions;

static class ResultsExtensions
{
    public static IResult NoCache(this IResultExtensions resultExtensions, CarouselDelivery cd)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);

        return new NoCacheResult(cd);
    }
}

class NoCacheResult : IResult
{
    private readonly CarouselDelivery _cd;

    public NoCacheResult(CarouselDelivery cd)
    {
        _cd = cd;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        // Force the reponse to not be cached at the browser
        httpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        httpContext.Response.Headers["Expires"] = "-1";
        httpContext.Response.Headers["Pragma"] = "no-cache";

        return httpContext.Response.WriteAsJsonAsync(_cd);
    }
}