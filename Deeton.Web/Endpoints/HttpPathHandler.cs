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

    public HttpSpecificPathHandler(IHttpPath path)
    {
        ContentTypeGetter = path.GetContentType;
        ContentGetter = path.GetContent;
        Subpath = path.SubPath;
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

    /// <summary>
    /// Make this <see cref="HttpSpecificPathHandler"/> listen on the sub-path supplied.
    /// The sub-path is the section that comes after the base of the URL. If this endpoint is on
    /// "/api/v1", with a sub-path of "version", it would listen on "/api/v1/version"
    /// </summary>
    /// <param name="subPath">The sub path relative to the base path supplied.</param>
    /// <returns>Self, to allow chaining.</returns>
    public HttpSpecificPathHandler WithSubPath(string subPath)
    {
        var newPathHandler = new HttpSpecificPathHandler(subPath);
        _handlers.Add(subPath, newPathHandler);
        return newPathHandler;
    }

    /// <summary>
    /// Use a custom <see cref="IHttpPath"/>. This will case <see cref="WithSubPath(string)"/> with
    /// <see cref="IHttpPath.SubPath"/>.
    /// </summary>
    /// <inheritdoc cref="WithSubPath(string)"/>
    /// <typeparam name="T"></typeparam>
    /// <returns>Self, to allow chaining.</returns>
    public HttpPathHandler WithSubPath<T>() where T: IHttpPath, new()
    {
        var inst = new T();
        WithSubPath(inst.SubPath)
            .WithContentTypeGetter(inst.GetContentType)
            .WithContentGetter(inst.GetContent);
        return this;
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

    internal Dictionary<string, HttpSpecificPathHandler> GetSubhandlers()
        => _handlers;
}
