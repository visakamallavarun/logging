namespace MiniLogging;

/// <summary>
/// Processes individual log events. This is the general contract every logger
/// implements; the LogInformation/LogWarning helpers are extension methods on top.
/// </summary>
public interface ILogger
{
    bool IsEnabled(LogLevel logLevel);

    void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter);

    IDisposable? BeginScope<TState>(TState state) where TState : notnull;
}

/// <summary>A category-specific logger. The category comes from <typeparamref name="TCategoryName"/>.</summary>
public interface ILogger<out TCategoryName> : ILogger
{
}
