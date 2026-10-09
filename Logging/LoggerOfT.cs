namespace MiniLogging;

/// <summary>
/// The generic logger DI hands to your services. It simply asks the factory for
/// the category logger named after <typeparamref name="T"/> and forwards to it.
/// </summary>
public sealed class Logger<T> : ILogger<T>
{
    private readonly ILogger _inner;

    public Logger(ILoggerFactory factory)
    {
        _inner = factory.CreateLogger(GetCategoryName());
    }

    public bool IsEnabled(LogLevel logLevel) => _inner.IsEnabled(logLevel);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
        => _inner.Log(logLevel, eventId, state, exception, formatter);

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => _inner.BeginScope(state);

    private static string GetCategoryName()
        => (typeof(T).FullName ?? typeof(T).Name).Replace('+', '.');
}
