
using Deeton.Web.Endpoints;
using System.Net;

namespace Deeton.Web;

public class WebSubsystem : IDisposable
{
    /// <summary>
    /// This is mapped by relative endpoint -> http listener.
    /// EG. (/api/v1/version -> WrapperImpl())
    /// </summary>
    private readonly Dictionary<string, HttpEndpointWrapper> _listeners = [];

    private readonly WebApplication _application;

    public WebSubsystem(WebApplication application)
    {
        _application = application;

#if DEBUG
        AddEndpoint("/_deeton", new DeetonSubEndpoint());
#endif
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

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        foreach (var (_, value) in _listeners)
        {
            value.Dispose();
        }
    }
}
