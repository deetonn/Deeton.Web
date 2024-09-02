using Deeton.Web;
using Newtonsoft.Json;

namespace Deeton.Web.Endpoints;

public class DeetonVersion
{
    public required int Major { get; init; }

    public required int Minor { get; init; }
}

/// <summary>
/// Get the version of Deeton.Web that is running.
/// 
/// This is only loaded in debug mode by default.
/// </summary>
public class DeetonSubEndpoint : IHttpEndpoint
{
    private Dictionary<string, Func<HttpRequestData, Content>> _subHandlers;

    public DeetonSubEndpoint()
    {
        _subHandlers = [];
        _subHandlers.Add("/version", (data) =>
        {
            return Content.FromText(JsonConvert.SerializeObject(new DeetonVersion()
            {
                Major = 0,
                Minor = 1
            }));
        });
    }

    public ContentType GetContentType(HttpRequestData data)
    {
        return Application.Json;
    }

    public Content Get(HttpRequestData data)
    {
        if (_subHandlers.TryGetValue(data.Subpath ?? string.Empty, out var func))
        {
            return func(data);
        }

        return Content.FromText("404 - Not Found!");
    }
}
