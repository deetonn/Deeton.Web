
using Spectre.Console;

namespace Deeton.Web;

/// <summary>
/// Closure that logs how long this object lived.
/// </summary>
/// <param name="what"></param>
internal class ExecutionTimer(string what) : IDisposable
{
    private readonly DateTime _start = DateTime.Now;
    private readonly string _what = what;

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (!DebugLogging.IsEnabled())
            return;

        var time = DateTime.Now - _start;
        var str = GetTimeString(time);

        DebugLogging.LogInfo($"[yellow]{_what}[/] took {str}");
    }

    public static string GetTimeString(TimeSpan ts)
    {
        if (ts.Hours > 0)
        {
            return $"[red bold]{ts.Hours}h[/]";
        }

        if (ts.Minutes > 0)
        {
            return $"[orange bold]{ts.Minutes}m {ts.Seconds}s[/]";
        }

        if (ts.Seconds > 0)
        {
            return $"[orange bold]{ts.Seconds}.{ts.Milliseconds}s[/]";
        }

        if (ts.Milliseconds > 0)
        {
            return $"[green bold]{ts.Milliseconds}.{ts.Microseconds}ms[/]";
        }

        if (ts.Microseconds > 0)
        {
            return $"[green bold]{ts.Microseconds}.{ts.Nanoseconds}us[/]";
        }

        return $"[green bold]{ts.Nanoseconds}ns[/]";
    }
}

/// <summary>
/// The logger that only works when debug is enabled.
/// </summary>
internal static class DebugLogging
{
    public static bool IsEnabled()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }

    public static void ClearStyles()
    {
        if (!IsEnabled())
            return;

        Console.Write("\x1b[0m");
    }

    public static void LogInfo(string message)
    {
        AnsiConsole.MarkupLine($"[#9CC185]info[/] : [white bold]{message}[/]");
    }

    public static void LogError(string message)
    {
        AnsiConsole.MarkupLine($"[#D2905E]error[/]: [white bold]{message}[/]");
    }

    public static void LogWarning(string message)
    {
        AnsiConsole.MarkupLine($"[#F08C33]warn[/] : [white bold]{message}[/]");
    }

    public static ExecutionTimer Timed(string what)
    {
        return new ExecutionTimer(what);
    }
}
