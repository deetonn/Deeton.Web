using System.Net;

namespace Deeton.Web;

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
        Listener = new HttpListener();
        Listener.Prefixes.Add(endpoint.AbsoluteEndpointUrl);

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
                Console.Error.WriteLine($"The HTTP listener for {endpoint.AbsoluteEndpointUrl} threw an exception.");
                Console.Error.WriteLine($" ^ {e.Message}");
            }
        });

        Endpoint = endpoint;
        HttpEndpoint = httpEndpoint;
        Listener.Start();
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
                Console.WriteLine("An invalid URL is being used for a HTTP endpoint.");
                Console.WriteLine($"Message: {e}");
                return;
            }
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

            Console.WriteLine($"GET {rawUrl.Text}");

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
