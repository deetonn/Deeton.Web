using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deeton.Web.Endpoints;

public class HttpSpecificPathHandler
{
    public Func<HttpRequestData, ContentType> ContentTypeGetter;
    public Func<HttpRequestData, Content> ContentGetter;

    public string Subpath;

    public HttpSpecificPathHandler(Func<HttpRequestData, ContentType> contentTypeGetter, Func<HttpRequestData, Content> contentGetter, string subPath)
    {
        ContentTypeGetter = contentTypeGetter;
        ContentGetter = contentGetter;
        Subpath = subPath;
    }

    public HttpSpecificPathHandler(string subPath)
    {
        ContentTypeGetter = (_) => Text.Html;
        ContentGetter = (_) => Content.FromText($"<html><h1>The handler for {Subpath} does not implement the getter.</h1></html>");
        Subpath = subPath;
    }

    public HttpSpecificPathHandler WithContentTypeGetter(Func<HttpRequestData, ContentType> contentTypeGetter)
    {
        ContentTypeGetter = contentTypeGetter;
        return this;
    }

    public HttpSpecificPathHandler WithContentGetter(Func<HttpRequestData, Content> contentGetter)
    {
        ContentGetter = contentGetter;
        return this;
    }
}

/// <summary>
/// This class is a helper that implements <see cref="IHttpEndpoint"/>. It allows you
/// to be able to listen on one http endpoint (such as /api) and respond to different URL paths
/// within that pattern.
/// </summary>
public class HttpPathHandler : IHttpEndpoint
{
    private readonly Dictionary<string, HttpSpecificPathHandler> _handlers;
    private readonly WebApplication _application;
    private readonly string _basePath;

    public HttpPathHandler(string basePath, WebApplication application)
    {
        _handlers = [];
        _application = application;
        _basePath = basePath;

        application.Subsystem.AddEndpoint(basePath, this);
    }

    public HttpSpecificPathHandler WithSubPath(string subPath)
    {
        var newPathHandler = new HttpSpecificPathHandler(subPath);
        _handlers.Add(subPath, newPathHandler);
        return newPathHandler;
    }

    public ContentType GetContentType(HttpRequestData data)
    {
        if (_handlers.TryGetValue(data.Subpath ?? "/", out var handler))
        {
            return handler.ContentTypeGetter(data);
        }

        var notFoundHandler = _application.GetNotFoundHandler();
        // Automatically attempt to redirect to /404
        return notFoundHandler(data).Item1;
    }

    public Content Get(HttpRequestData data)
    {
        if (_handlers.TryGetValue(data.Subpath ?? "/", out var handler))
        {
            return handler.ContentGetter(data);
        }

        var notFoundHandler = _application.GetNotFoundHandler();
        // Automatically attempt to redirect to /404
        return notFoundHandler(data).Item2;
    }
}
