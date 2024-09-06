
namespace Deeton.Web;

/// <summary>
/// The base content type.
/// </summary>
public record ContentType
{
    /// <summary>
    /// Automatically creates a valid "Content-Type: XXX" string from the types using 
    /// reflection.
    /// </summary>
    /// <returns></returns>
    public string AsHttpString()
    {
        var baseName = GetType().BaseType!.Name.ToLower();
        var ourName = GetType().Name.ToLower();

        return $"{baseName}/{ourName}";
    }
}

/// <summary>
/// Any content-type that lives with the "application/" realm.
/// </summary>
public record Application : ContentType
{
    /// <summary>
    /// Content-Type: application/json
    /// </summary>
    public static Json Json => new();

    /// <summary>
    /// Content-Type: application/ogg
    /// </summary>
    public static Ogg Ogg => new();

    /// <summary>
    /// Content-Type: application/pdf
    /// </summary>
    public static Pdf Pdf => new();
}
public record Json : Application;
public record Ogg : Application;
public record Pdf : Application;

public record Media : ContentType;

public record Text : ContentType
{
    public static Html Html => new();
    public static Css Css => new();
    public static Javascript Javascript => new();
    public static Xml Xml => new();
}

public record Html : Text;
public record Css : Text;
public record Javascript : Text;
public record Xml : Text;
