using System.Net;

namespace Deeton.Web;

public class WebApplication : IDisposable
{
    public readonly string Domain;
    public readonly WebSubsystem Subsystem;

    private bool _closing;

    private Func<HttpRequestData, (ContentType, Content)> _404Handler;

    /// <summary>
    /// Create a new web application instance, that listens on the domain and port supplied.
    /// 
    /// This argument to this should be a (DOMAIN:PORT) format. Please note that DOMAIN can be
    /// localhost. If you want to the server to be public, you must port forward PORT.
    /// </summary>
    /// <param name="domain">The domain. If you were hosting example.com on port 6969. You would set this argument to "example.com:6969". Please note http(s):// is prepeded for you.</param>
    /// <exception cref="NotSupportedException">This exception can occur when <see cref="HttpListener.IsSupported"/> returns false.</exception>
    public WebApplication(string domain)
    {
        _closing = false;

        using var timer = DebugLogging.Timed("Initialization of the WebApplication");

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
    /// Begin waiting for incoming requests. This function merely stops the main thread
    /// from exiting and must be called there.
    /// 
    /// If you wish to block the main thread from exiting another way, do not call this function,
    /// the http listeners will still be listening, as they are ran on a thread pool.
    /// </summary>
    public void Listen()
    {
        SpinWait.SpinUntil(() => _closing);
    }

    /// <summary>
    /// Cause the main thread to unblock, which will likely cause the application to close.
    /// 
    /// It is always recommended that when you declare your instance of <see cref="WebApplication"/>,
    /// you use a "using" declaration, so that all the HTTP listeners are disposed of correctly.
    /// </summary>
    public void Shutdown()
    {
        _closing = true;
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
        Shutdown();
        Subsystem.Dispose();
    }
}
