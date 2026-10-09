namespace MiniLogging;

/// <summary>Surface configured by your AddLogging callback.</summary>
public interface ILoggingBuilder
{
    IList<ILoggerProvider> Providers { get; }
    LoggerFilterOptions FilterOptions { get; }
}

internal sealed class LoggingBuilder : ILoggingBuilder
{
    public IList<ILoggerProvider> Providers { get; } = [];
    public LoggerFilterOptions FilterOptions { get; } = new();
}
