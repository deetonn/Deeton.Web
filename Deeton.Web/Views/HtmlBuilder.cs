
using System.Text;

namespace Deeton.Web.Views;

// TODO: not ready yet.
internal class HtmlElementInserter : IDisposable
{
    private string _elementName;
    private string _content;
    private HtmlBuilder _builder;
    private string? _elementArgs;
    private bool _disposed = false;

    public HtmlElementInserter(string el, string content, HtmlBuilder builder)
    {
        _elementName = el;
        _content = content;
        _builder = builder;

        _builder.GetInnerContent().AppendLine($"<{el}{GetElementArgString()}>");
        _builder.GetInnerContent().AppendLine($"{content}");
    }

    public HtmlElementInserter WithElementArguments(string arguments)
    {
        _elementArgs = arguments;
        return this;
    }

    public void CloseEarly()
    {
        Dispose();
    }

    public string GetElementArgString()
    {
        if (_elementArgs != null)
        {
            return " " + _elementArgs;
        }

        return string.Empty;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            GC.SuppressFinalize(this);

            _builder.GetInnerContent().AppendLine($"</{_elementName}>");
        }
    }
}

internal class HtmlBuilder : Content
{
    private List<HtmlElementInserter> _inserters;

    public HtmlBuilder()
    {
        _inserters = [];
    }

    public HtmlBuilder Push(string elementType, string? content = null)
    {
        _inserters.Add(new HtmlElementInserter(elementType, content ?? string.Empty, this));
        return this;
    }

    public StringBuilder GetInnerContent() => base.ContentBytes;

    public override byte[] GetBytes(Encoding? encoding = null)
    {
        foreach (var item in _inserters.Reverse<HtmlElementInserter>())
        {
            // We have to insert closing tags in reverse.
            item.Dispose();
        }
        return base.GetBytes(encoding);
    }
}
