
namespace Deeton.Web;

public class HttpRequestData
{
    /// <summary>
    /// The headers associated with this request.
    /// </summary>
    public required HttpHeaders Headers { get; init; }

    /// <summary>
    /// The path that is lower than the specified endpoint. Due to the actual Http listener
    /// listening to anything after and including the specified prefix, we can also provide
    /// further information.
    /// 
    /// Let's say there's a listener on "/api/v1". If there's a request made to 
    /// "/api/v1/version", the handler for "/api/v1" would receive the request, and this 
    /// propery would be "/version".
    /// </summary>
    public required string? Subpath { get; init; }
}