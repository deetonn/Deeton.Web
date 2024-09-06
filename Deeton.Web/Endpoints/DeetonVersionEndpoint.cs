using Deeton.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Deeton.Web.Endpoints;

internal class DeetonVersion
{
    public required int Major { get; init; }

    public required int Minor { get; init; }
}

/// <summary>
/// Get the version of Deeton.Web that is running.
/// 
/// This is only loaded in debug mode by default.
/// </summary>
internal class DeetonSubEndpoint
{
    private static WebApplication? _app;

    public static HttpPathHandler Load(WebApplication application)
    {
        _app ??= application;
        var handler = new HttpPathHandler("/_deeton", application);

        handler
            .WithSubPath("/version")
            .WithContentTypeGetter((_) => Application.Json)
            .WithContentGetter((_) => Content.FromText(
                JsonConvert.SerializeObject(
                    new DeetonVersion { Major = 0, Minor = 1 }
                    )
                )
            );

        handler
            .WithSubPath("/endpoints")
            .WithContentTypeGetter((_) => Application.Json)
            .WithContentGetter(GetEndpoints);

        return handler;
    }

    private static Content GetEndpoints(HttpRequestData data)
    {
        var listeners = _app?.Subsystem._listeners;
        List<EndpointData> endpoints = [];

        if (listeners is null)
        {
            // Empty JSON object. There are no endpoints currently.
            return Content.FromObjectIntoJson(
                new EndpointDataResult { Data = endpoints }
            );
        }

        foreach (var (path, listener) in listeners)
        {
            if (listener.HttpEndpoint is HttpPathHandler ph)
            {
                var sh = ph.GetSubhandlers();
                foreach (var (subpath, subhandler) in sh)
                {
                    endpoints.Add(new EndpointData()
                    {
                        Path = path + subpath,
                        ContentType = subhandler.ContentTypeGetter(
                            new HttpRequestData
                            {
                                Headers = new(new Dictionary<string, string>()),
                                Subpath = subpath
                            }
                        ).AsHttpString()
                    });
                }
            }
            else
            {
                endpoints.Add(new EndpointData
                {
                    Path = path,
                    ContentType = listener.HttpEndpoint.GetContentType(new HttpRequestData
                    {
                        Headers = new(new Dictionary<string, string>()),
                        Subpath = path
                    }).AsHttpString()
                });
            }
        }

        return Content.FromObjectIntoJson(new EndpointDataResult { Data = endpoints });
    }
}

internal class EndpointData
{
    [JsonProperty(PropertyName = "path")]
    public string? Path { get; set; }

    [JsonProperty(PropertyName = "contentType")]
    public string? ContentType { get; set; }
}

internal class EndpointDataResult
{
    [JsonProperty(PropertyName = "data")]
    public List<EndpointData>? Data { get; set; }
}
