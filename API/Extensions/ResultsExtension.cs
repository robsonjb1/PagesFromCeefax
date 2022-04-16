using System.Net.Mime;
using System.Text;

static class ResultsExtensions
{
    public static IResult NoCache(this IResultExtensions resultExtensions, string html)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);

        return new NoCacheResult(html);
    }
}

class NoCacheResult : IResult   
{
    private readonly string _html;

    public NoCacheResult(string html)
    {
        _html = html;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        // Force the reponse to not be cached at the browser
        httpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        httpContext.Response.Headers["Expires"] = "-1";
        httpContext.Response.Headers["Pragma"] = "no-cache";

        return httpContext.Response.WriteAsync(_html);
    }
}
