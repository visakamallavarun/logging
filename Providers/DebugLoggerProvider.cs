using System.Diagnostics;

namespace MiniLogging.Providers;

/// <summary>Creates loggers that write to the debugger output via System.Diagnostics.Debug.</summary>
public sealed class DebugLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new DebugLogger(categoryName);

    public void Dispose() { }
}

internal sealed class DebugLogger : ILogger
{
    private readonly string _category;

    public DebugLogger(string category) => _category = category;

    public bool IsEnabled(LogLevel logLevel) => Debugger.IsAttached;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        string message = formatter(state, exception);
        Debug.WriteLine($"{logLevel}: {_category} - {message}");
        if (exception != null)
            Debug.WriteLine(exception);
    }
}
