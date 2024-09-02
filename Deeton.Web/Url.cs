
namespace Deeton.Web;

/// <summary>
/// A URL. (Uniform resource locator)
/// </summary>
public class Url
{
    /// <summary>
    /// The arguments provided in the URL, not the HTTP headers.
    /// </summary>
    public Dictionary<string, string> Arguments { get; }

    /// <summary>
    /// The actual URL resource.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Construct a new <see cref="Url"/> instance, picking the available information apart
    /// and allowing its different component to be viewed individually.
    /// </summary>
    /// <param name="url"></param>
    /// <exception cref="Exception"></exception>
    public Url(string url)
    {
        Arguments = [];
        var arguments = url.Split('?');
        Text = arguments.First();

        if (arguments.Length >= 2)
        {
            var parts = arguments[1];
            foreach (var part in parts.Split('&'))
            {
                var bits = part.Split('=');
                var (key, value) = (bits[0], bits[1]);
                Arguments.Add(key, value);
            }
        }
    }
}