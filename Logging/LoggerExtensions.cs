namespace MiniLogging;

/// <summary>
/// Convenience helpers that funnel every call through the single
/// ILogger.Log method, exactly like the real framework does.
/// </summary>
public static class LoggerExtensions
{
    public static void LogTrace(this ILogger logger, string message, params object?[] args)
        => logger.Log(LogLevel.Trace, message, args);

    public static void LogDebug(this ILogger logger, string message, params object?[] args)
        => logger.Log(LogLevel.Debug, message, args);

    public static void LogInformation(this ILogger logger, string message, params object?[] args)
        => logger.Log(LogLevel.Information, message, args);

    public static void LogWarning(this ILogger logger, string message, params object?[] args)
        => logger.Log(LogLevel.Warning, message, args);

    public static void LogError(this ILogger logger, string message, params object?[] args)
        => logger.Log(LogLevel.Error, null, message, args);

    public static void LogError(this ILogger logger, Exception exception, string message, params object?[] args)
        => logger.Log(LogLevel.Error, exception, message, args);

    public static void LogCritical(this ILogger logger, string message, params object?[] args)
        => logger.Log(LogLevel.Critical, null, message, args);

    public static void Log(this ILogger logger, LogLevel logLevel, string message, params object?[] args)
        => logger.Log(logLevel, null, message, args);

    public static void Log(this ILogger logger, LogLevel logLevel, Exception? exception, string message, params object?[] args)
        => logger.Log(logLevel, 0, new FormattedLogValues(message, args), exception, FormattedLogValues.Formatter);

    /// <summary>Begins a scope from a message template, e.g. "Processing {EmployeeId}".</summary>
    public static IDisposable? BeginScope(this ILogger logger, string messageFormat, params object?[] args)
        => logger.BeginScope(new FormattedLogValues(messageFormat, args));
}
