using System.Net;

namespace Deeton.Web;

/// <summary>
/// This class is the wrapper for a HTTP listener. It takes all supplied information and actually
/// listens and waits for a specific HTTP path and calls relevant functions such as <see cref="IHttpEndpoint.Get(Deeton.Web.HttpRequestData)"/>
/// and <see cref="IHttpEndpoint.Set(Deeton.Web.HttpRequestData)"/>
/// </summary>
public class HttpEndpointWrapper : IDisposable
{
    /// <summary>
    /// The actual HTTP listener that will listen for requests on a certain endpoint.
    /// </summary>
    public HttpListener? Listener { get; set; }

    /// <summary>
    /// The endpoint string.
    /// </summary>
    public MappedEndpointText Endpoint { get; set; }

    /// <summary>
    /// The class that implements <see cref="IHttpEndpoint"/> that will respond.
    /// </summary>
    public IHttpEndpoint HttpEndpoint { get; private set; }

    /// <summary>
    /// Construct the wrapper that deals with create a HTTP listener and calling
    /// the <see cref="IHttpEndpoint"/> object when requests are made.
    /// 
    /// This object is also responsible for cleaning up once this endpoint is done.
    /// </summary>
    /// <inheritdoc cref="HttpListener"/>
    /// <inheritdoc cref="ThreadPool.QueueUserWorkItem(WaitCallback)"/>
    /// <param name="endpoint"></param>
    /// <param name="httpEndpoint"></param>
    public HttpEndpointWrapper(MappedEndpointText endpoint, IHttpEndpoint httpEndpoint)
    {
        using var timer = DebugLogging.Timed($"Initializing HTTP endpoint for [yellow]{endpoint.RelativeEndpointUrl}[/]");

        Listener = new HttpListener();
        Listener.Prefixes.Add(endpoint.AbsoluteEndpointUrl);

        // Must call this before we queue the thread job.
        Listener.Start();

        ThreadPool.QueueUserWorkItem(async _ =>
        {
            try
            {
                while (true)
                {
                    await CoreThreadFunction();
                }
            }
            catch (Exception e)
            {
                DebugLogging.LogError($"The HTTP listening thread threw an exception. (for [yellow]{endpoint.AbsoluteEndpointUrl}[/]");
                DebugLogging.LogError($"Further information on the error above: {e.Message}");
            }
        });

        Endpoint = endpoint;
        HttpEndpoint = httpEndpoint;
    }

    private async Task CoreThreadFunction()
    {
        var context = await Listener!.GetContextAsync();

        if (context.Request.HttpMethod == "GET")
        {
            Url rawUrl;
            try
            {
                rawUrl = new Url(context!.Request.RawUrl!);
            }
            catch (Exception e)
            {
                DebugLogging.LogError($"Failed to parse URL during a GET request for [yellow]{Endpoint.RelativeEndpointUrl}[/]");
                DebugLogging.LogError($"Further information regarding the previous error: {e}");
                return;
            }
            using var getTimer = DebugLogging.Timed($"Serving a GET request for [yellow]{rawUrl.Text}[/]");

            var headers = new HttpHeaders(context!.Request.Headers);
            headers.JoinWith(rawUrl.Arguments);

            var subPath = rawUrl.Text.Replace(Endpoint.RelativeEndpointUrl, string.Empty);

            var httpData = new HttpRequestData
            {
                Headers = headers,
                Subpath = subPath
            };

            var contentType = HttpEndpoint.GetContentType(httpData);
            var requestContent = HttpEndpoint.Get(httpData) ?? throw new NotImplementedException($"Handle HTTP/Get not being implemented!");
            context.Response.ContentType = contentType.AsHttpString();
            context.Response.StatusCode = 200;
            var bytes = requestContent.GetBytes();
            context.Response.ContentLength64 = bytes.LongLength;
            await context.Response.OutputStream.WriteAsync(bytes);
            context.Response.OutputStream.Close();

            // DebugLogging.LogInfo($"HTTP/GET -> {rawUrl.Text} (Responded with {bytes.LongLength} bytes)");

            return;
        }

        throw new NotImplementedException($"Http/{context.Request.HttpMethod} is not supported!");
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        Listener?.Stop();
    }
}
