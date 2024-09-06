
namespace Deeton.Web.Endpoints;

public interface IHttpPath
{
    /// <summary>
    /// The sub path this HttpPath represents. For example,
    /// "/assets/bundle.js"
    /// </summary>
    public string SubPath { get; }

    /// <summary>
    /// A function that will return the correct <see cref="ContentType"/> based on the <see cref="HttpRequestData"/>
    /// that is passed in as an argument.
    /// </summary>
    /// <param name="data">The HTTP request data for this specific GET request.</param>
    /// <returns>The content type.</returns>
    public ContentType GetContentType(HttpRequestData data);

    /// <summary>
    /// A function that will return web content. The content returned must match with the content type returned
    /// by <see cref="GetContentType(HttpRequestData)"/>.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Content GetContent(HttpRequestData data);
}