
namespace Deeton.Web;

public interface IHttpEndpoint
{
    /// <summary>
    /// Get the type of content this endpoint will return.
    /// </summary>
    /// <returns>The content type. Such as <see cref="Application.Json"/> for example.</returns>
    public ContentType GetContentType(HttpRequestData data);

    /// <summary>
    /// This function is called when the request type is GET.
    /// </summary>
    /// <param name="headers">The headers that were contained in this request.</param>
    /// <returns>The content, or null if there is no content here.</returns>
    public Content? Get(HttpRequestData headers)
    {
        return null;
    }
}
