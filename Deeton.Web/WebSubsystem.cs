
using Deeton.Web.Endpoints;
using System.Net;

namespace Deeton.Web;

public class WebSubsystem : IDisposable
{
    /// <summary>
    /// This is mapped by relative endpoint -> http listener.
    /// EG. (/api/v1/version -> WrapperImpl())
    /// </summary>
    internal readonly Dictionary<string, HttpEndpointWrapper> _listeners = [];

    /// <summary>
    /// The parent web application.
    /// </summary>
    private readonly WebApplication _application;

    public WebSubsystem(WebApplication application)
    {
        using var timer = DebugLogging.Timed("Initialization of the WebSubsystem");

        _application = application;
    }

    /// <summary>
    /// Add a simple endpoint. When <paramref name="relativeEndpointPath"/> is hit on a HTTP request,
    /// <paramref name="endpoint"/> will be used to respond.
    /// </summary>
    /// <param name="relativeEndpointPath">The relative URL path. Such as /api/v1/...</param>
    /// <param name="endpoint">The endpoint to be used</param>
    public void AddEndpoint(string relativeEndpointPath, IHttpEndpoint endpoint)
    {
        var mappedEpText = new MappedEndpointText(_application.Domain, relativeEndpointPath);
        _listeners.Add(mappedEpText.AbsoluteEndpointUrl, new HttpEndpointWrapper(mappedEpText, endpoint));
    }

    /// <summary>
    /// Get an <see cref="IHttpEndpoint"/> that corresponds to the relative URL path.
    /// </summary>
    /// <param name="relativeEndpointPath"></param>
    /// <returns></returns>
    public IHttpEndpoint? GetEndpoint(string relativeEndpointPath)
    {
        var mappedEpText = new MappedEndpointText(_application.Domain, relativeEndpointPath);
        if (!_listeners.TryGetValue(mappedEpText.AbsoluteEndpointUrl, out var wrapper))
        {
            return null;
        }

        return wrapper.HttpEndpoint;
    }

    /// <summary>
    /// Loads the "_deeton" endpoint that provides version info etc.
    /// 
    /// So far, this provides "/_deeton/version" and "/_deeton/endpoints".
    /// </summary>
    public void LoadBuiltinEndpoint()
    {
        DeetonSubEndpoint.Load(_application);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        using var timer = DebugLogging.Timed("WebSubsystem shutdown");

        foreach (var (_, value) in _listeners)
        {
            value.Dispose();
        }
    }
}
