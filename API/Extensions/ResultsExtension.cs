using System.Net.Mime;
using System.Text;
static class ResultsExtensions
{
    public static IResult NoCache(this IResultExtensions resultExtensions, StringBuilder html)
    {
        ArgumentNullException.ThrowIfNull(resultExtensions);

        return new NoCacheResult(html.ToString());
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
        httpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        httpContext.Response.Headers["Expires"] = "-1";
        httpContext.Response.Headers["Pragma"] = "no-cache";

        return httpContext.Response.WriteAsync(_html);
    }
}
