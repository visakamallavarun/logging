using System.Collections.Concurrent;
using System.Text;

namespace MiniLogging.Providers;

/// <summary>Creates loggers that write to the console. One scope stack is shared per provider.</summary>
public sealed class ConsoleLoggerProvider : ILoggerProvider
{
    private readonly LoggerExternalScopeProvider _scopes = new();
    private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers = new();

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName, name => new ConsoleLogger(name, _scopes));

    public void Dispose() => _loggers.Clear();
}

internal sealed class ConsoleLogger : ILogger
{
    private static readonly object ConsoleLock = new();

    private readonly string _category;
    private readonly LoggerExternalScopeProvider _scopes;

    public ConsoleLogger(string category, LoggerExternalScopeProvider scopes)
    {
        _category = category;
        _scopes = scopes;
    }

    public bool IsEnabled(LogLevel logLevel) => true; // Filtering already handled by the factory logger.

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _scopes.Push(state);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        string message = formatter(state, exception);

        var scopeText = new StringBuilder();
        _scopes.ForEachScope((scope, sb) => sb.Append(" => ").Append(scope), scopeText);

        lock (ConsoleLock)
        {
            ConsoleColor previous = Console.ForegroundColor;
            Console.ForegroundColor = ColorFor(logLevel);
            Console.Write($"{ShortName(logLevel)}: ");
            Console.ForegroundColor = previous;

            Console.WriteLine($"{_category}[{eventId.Id}]{scopeText}");
            Console.WriteLine($"      {message}");
            if (exception != null)
                Console.WriteLine(exception);
        }
    }

    private static string ShortName(LogLevel level) => level switch
    {
        LogLevel.Trace => "trce",
        LogLevel.Debug => "dbug",
        LogLevel.Information => "info",
        LogLevel.Warning => "warn",
        LogLevel.Error => "fail",
        LogLevel.Critical => "crit",
        _ => "none"
    };

    private static ConsoleColor ColorFor(LogLevel level) => level switch
    {
        LogLevel.Trace => ConsoleColor.Gray,
        LogLevel.Debug => ConsoleColor.Gray,
        LogLevel.Information => ConsoleColor.Green,
        LogLevel.Warning => ConsoleColor.Yellow,
        LogLevel.Error => ConsoleColor.Red,
        LogLevel.Critical => ConsoleColor.Magenta,
        _ => ConsoleColor.White
    };
}
