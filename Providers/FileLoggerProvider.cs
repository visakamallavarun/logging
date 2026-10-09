using System.Collections.Concurrent;
using System.Text;

namespace MiniLogging.Providers;

/// <summary>
/// Creates loggers that append to a file. The provider owns the stream and
/// flushes/closes it on Dispose, which is why provider lifetime is managed.
/// </summary>
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly LoggerExternalScopeProvider _scopes = new();
    private readonly StreamWriter _writer;
    private readonly object _writeLock = new();
    private bool _disposed;

    public FileLoggerProvider(string path)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        _writer = new StreamWriter(path, append: true) { AutoFlush = false };
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(categoryName, _scopes, WriteLine);

    private void WriteLine(string line)
    {
        lock (_writeLock)
        {
            if (_disposed)
                return;

            _writer.WriteLine(line);
        }
    }

    public void Dispose()
    {
        lock (_writeLock)
        {
            if (_disposed)
                return;

            _disposed = true;
            _writer.Flush();
            _writer.Dispose();
        }
    }
}

internal sealed class FileLogger : ILogger
{
    private readonly string _category;
    private readonly LoggerExternalScopeProvider _scopes;
    private readonly Action<string> _write;

    public FileLogger(string category, LoggerExternalScopeProvider scopes, Action<string> write)
    {
        _category = category;
        _scopes = scopes;
        _write = write;
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _scopes.Push(state);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var sb = new StringBuilder();
        sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
          .Append(" [").Append(logLevel).Append("] ")
          .Append(_category);

        _scopes.ForEachScope((scope, builder) => builder.Append(" => ").Append(scope), sb);

        sb.Append(" - ").Append(formatter(state, exception));
        if (exception != null)
            sb.Append(" | ").Append(exception);

        _write(sb.ToString());
    }
}
