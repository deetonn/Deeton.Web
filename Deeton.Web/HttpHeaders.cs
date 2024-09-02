
using System.Diagnostics.CodeAnalysis;

namespace Deeton.Web;

/// <summary>
/// Headers that are send VIA a http request.
/// </summary>
public class HttpHeaders
{
    private readonly Dictionary<string, string> _headers = [];

    public HttpHeaders(System.Net.Http.Headers.HttpHeaders headers)
    {
        foreach (var (key, values) in headers)
        {
            _headers.TryAdd(key, values.First());
        }
    }
    public HttpHeaders(System.Collections.Specialized.NameValueCollection headers)
    {
        foreach (string key in headers)
        {
            var value = headers.GetValues(key);
            var firstValue = value?.FirstOrDefault();

            if (firstValue is null)
                continue;

            _headers.Add(key, firstValue);
        }
    }
    public HttpHeaders(Dictionary<string, string> headers)
    {
        _headers = headers;
    }

    public void JoinWith(Dictionary<string, string> otherHeaders)
    {
        foreach (var (k, v) in otherHeaders)
            _headers.TryAdd(k, v);
    }

    public string? TryGet(string key)
    {
        if (_headers.TryGetValue(key, out var value)) return value;
        return null;
    }

    public string? this[string key]
    {
        get
        {
            if (!_headers.TryGetValue(key, out var value))
                return null;
            return value;
        }
    }
}
