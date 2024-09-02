using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deeton.Web;

public class MappedEndpointText
{
    public readonly string AbsoluteEndpointUrl;
    public readonly string RelativeEndpointUrl;

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
