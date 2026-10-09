namespace MiniLogging;

/// <summary>Pairs a provider's logger with the provider type that created it.</summary>
internal readonly struct LoggerInformation
{
    public LoggerInformation(Type providerType, ILogger logger)
    {
        ProviderType = providerType;
        Logger = logger;
    }

    public Type ProviderType { get; }
    public ILogger Logger { get; }
}

/// <summary>
/// The factory-managed category logger. It evaluates filtering once, then
/// dispatches an eligible event to every provider's logger for that category.
/// </summary>
internal sealed class Logger : ILogger
{
    private readonly string _category;
    private readonly LoggerInformation[] _loggers;
    private readonly LoggerFilterOptions _filter;

    public Logger(string category, LoggerInformation[] loggers, LoggerFilterOptions filter)
    {
        _category = category;
        _loggers = loggers;
        _filter = filter;
    }

    public bool IsEnabled(LogLevel logLevel)
        => logLevel != LogLevel.None && logLevel >= _filter.GetMinLevel(_category);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        foreach (var info in _loggers)
            info.Logger.Log(logLevel, eventId, state, exception, formatter);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var scopes = new List<IDisposable>(_loggers.Length);
        foreach (var info in _loggers)
        {
            var scope = info.Logger.BeginScope(state);
            if (scope != null)
                scopes.Add(scope);
        }

        return new CompositeDisposable(scopes);
    }

    private sealed class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables;

        public CompositeDisposable(List<IDisposable> disposables) => _disposables = disposables;

        public void Dispose()
        {
            for (int i = _disposables.Count - 1; i >= 0; i--)
                _disposables[i].Dispose();
        }
    }
}
