using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deeton.Web;

/// <summary>
/// Contains information about an endpoint that is registered with a HTTP listener.
/// 
/// This includes information about it's relative path that was provided, and an alternate absolute path
/// that is derived from this relative path.
/// </summary>
public class MappedEndpointText
{
    /// <summary>
    /// The absolute URL for this endpoint.
    /// </summary>
    public readonly string AbsoluteEndpointUrl;

    /// <summary>
    /// The relative URL for this endpoint. (What was supplied by the user)
    /// </summary>
    public readonly string RelativeEndpointUrl;

    /// <summary>
    /// Automatically generate absolute and relative URL's for an endpoint.
    /// 
    /// This deals with loose slashes.
    /// </summary>
    /// <param name="baseUrl">The base URL, like: example.com:6969/</param>
    /// <param name="relativeEpUrl">The relative URL, like: /api/v1/version</param>
    /// <exception cref="UriFormatException"></exception>
    public MappedEndpointText(string baseUrl, string relativeEpUrl)
    {
        RelativeEndpointUrl = relativeEpUrl;

        if (!baseUrl.EndsWith('/'))
            baseUrl += '/';
        if (relativeEpUrl.StartsWith('/'))
            relativeEpUrl = relativeEpUrl[1..];

        AbsoluteEndpointUrl = baseUrl + relativeEpUrl;
        if (!Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute))
        {
            throw new UriFormatException($"The URI '{relativeEpUrl}' is malformed.");
        }

        if (!AbsoluteEndpointUrl.EndsWith('/'))
            AbsoluteEndpointUrl += '/';
    }
}
