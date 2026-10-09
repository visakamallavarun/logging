namespace MiniLogging;

/// <summary>Severity of a log event. Mirrors Microsoft.Extensions.Logging.LogLevel.</summary>
public enum LogLevel
{
    Trace,
    Debug,
    Information,
    Warning,
    Error,
    Critical,
    None
}
