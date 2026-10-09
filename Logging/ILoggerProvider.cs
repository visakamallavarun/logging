namespace MiniLogging;

/// <summary>
/// Creates destination-specific loggers (console, file, debug, ...).
/// A provider owns the resources of its destination, so it is IDisposable.
/// </summary>
public interface ILoggerProvider : IDisposable
{
    ILogger CreateLogger(string categoryName);
}
