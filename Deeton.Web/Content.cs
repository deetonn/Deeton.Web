
using Newtonsoft.Json;
using System.Text;

namespace Deeton.Web;

/// <summary>
/// The generic view of any content sent over HTTP.
/// </summary>
public class Content
{
    protected StringBuilder ContentBytes;

    public Content()
    {
        ContentBytes = new StringBuilder();
    }

    /// <summary>
    /// Get the contents as an array of bytes. 
    /// </summary>
    /// <param name="encoding">The encoding of the bytes. This is defaulted to <see cref="Encoding.UTF8"/></param>
    /// <returns>The array of bytes, encoded as <paramref name="encoding"/></returns>
    public virtual byte[] GetBytes(Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return encoding.GetBytes(ContentBytes.ToString());
    }

    /// <summary>
    /// Modify the content.
    /// </summary>
    /// <param name="modifier">The function that will apply this modification.</param>
    /// <returns>A new <see cref="Content"/> instance, with the modified contents</returns>
    public Content Modify(Func<StringBuilder, string> modifier)
    {
        var newContent = modifier(new StringBuilder(ContentBytes.ToString()));
        return FromText(newContent);
    }

    /// <summary>
    /// Create a new <see cref="Content"/> instance from text.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Content FromText(string text)
    {
        var content = new Content
        {
            ContentBytes = new StringBuilder(text)
        };
        return content;
    }

    /// <summary>
    /// Serialize an object and turn the text JSON into a <see cref="Content"/> instance.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Content FromObjectIntoJson(object? obj)
    {
        var jsonContent = JsonConvert.SerializeObject(obj);
        var content = new Content
        {
            ContentBytes = new StringBuilder(jsonContent)
        };
        return content;
    }

    /// <summary>
    /// Create a new <see cref="Content"/> instance from a file.
    /// 
    /// This will read the file contents.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <inheritdoc cref="File.ReadAllLines(string)"/>
    /// <returns></returns>
    public static Content FromFile(string path)
    {
        var lines = File.ReadAllLines(path);
        var finalContent = string.Join(Environment.NewLine, lines);
        return FromText(finalContent);
    }
}
