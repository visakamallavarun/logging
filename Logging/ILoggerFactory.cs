namespace MiniLogging;

/// <summary>
/// Central component that coordinates registered providers, creates category
/// loggers, caches them and applies filtering configuration.
/// </summary>
public interface ILoggerFactory : IDisposable
{
    ILogger CreateLogger(string categoryName);
    void AddProvider(ILoggerProvider provider);
}
