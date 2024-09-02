using System.Net;

namespace Deeton.Web;

public class WebApplication : IDisposable
{
    public readonly string Domain;
    public readonly WebSubsystem Subsystem;

    private Func<HttpRequestData, (ContentType, Content)> _404Handler;

    public WebApplication(string domain)
    {
        if (!domain.Contains("http") || !domain.Contains("https"))
        {
            // default to http.
            domain = $"http://{domain}";
        }

        Domain = domain;
        if (!HttpListener.IsSupported)
        {
            throw new NotSupportedException($"The class HttpListener is not supported on this platform.");
        }

        _404Handler = Default404Handler;
        Subsystem = new WebSubsystem(this);
    }

    /// <summary>
    /// the Not Found (404) handler for this application.
    /// 
    /// This function simply returns a Content-Type and some content that will be
    /// displayed whenever the user navigates somewhere that doesn't exist.
    /// </summary>
    /// <returns></returns>
    public Func<HttpRequestData, (ContentType, Content)> GetNotFoundHandler()
    {
        return _404Handler;
    }

    /// <summary>
    /// Set
    /// <inheritdoc cref="GetNotFoundHandler"/>
    /// </summary>
    /// <param name="newHandler"></param>
    public void SetNotFoundHandler(Func<HttpRequestData, (ContentType, Content)> newHandler)
    {
        _404Handler = newHandler;
    }

    private (ContentType, Content) Default404Handler(HttpRequestData data)
    {
        return (Text.Html, Content.FromText($@"
<html>
    <h1>404 - Page not found</h1>
    <p>The page {data.Subpath} was not found on this webserver.</p>
</html>
"));
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
