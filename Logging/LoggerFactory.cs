using System.Collections.Concurrent;

namespace MiniLogging;

/// <summary>
/// Coordinates the registered providers. For each category it builds one logger
/// per provider, wraps them in a composite <see cref="Logger"/>, and caches the
/// result so repeated resolutions reuse the same infrastructure.
/// </summary>
public sealed class LoggerFactory : ILoggerFactory
{
    private readonly List<ILoggerProvider> _providers;
    private readonly LoggerFilterOptions _filter;
    private readonly ConcurrentDictionary<string, Logger> _loggers = new();
    private bool _disposed;

    public LoggerFactory(IEnumerable<ILoggerProvider> providers, LoggerFilterOptions filter)
    {
        _providers = providers.ToList();
        _filter = filter;
    }

    public void AddProvider(ILoggerProvider provider) => _providers.Add(provider);

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName, CreateLoggerImpl);

    private Logger CreateLoggerImpl(string category)
    {
        var infos = new LoggerInformation[_providers.Count];
        for (int i = 0; i < _providers.Count; i++)
        {
            var provider = _providers[i];
            infos[i] = new LoggerInformation(provider.GetType(), provider.CreateLogger(category));
        }

        return new Logger(category, infos, _filter);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        foreach (var provider in _providers)
            provider.Dispose();
    }
}
