using MiniLogging.Providers;

namespace MiniLogging;

/// <summary>
/// The fluent methods you call inside AddLogging(...): register/clear providers
/// (destinations) and configure filtering.
/// </summary>
public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder ClearProviders(this ILoggingBuilder builder)
    {
        builder.Providers.Clear();
        return builder;
    }

    public static ILoggingBuilder AddConsole(this ILoggingBuilder builder)
    {
        builder.Providers.Add(new ConsoleLoggerProvider());
        return builder;
    }

    public static ILoggingBuilder AddDebug(this ILoggingBuilder builder)
    {
        builder.Providers.Add(new DebugLoggerProvider());
        return builder;
    }

    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string path)
    {
        builder.Providers.Add(new FileLoggerProvider(path));
        return builder;
    }

    public static ILoggingBuilder SetMinimumLevel(this ILoggingBuilder builder, LogLevel level)
    {
        builder.FilterOptions.MinLevel = level;
        return builder;
    }

    public static ILoggingBuilder AddFilter(this ILoggingBuilder builder, string category, LogLevel level)
    {
        builder.FilterOptions.Rules.Add(new LoggerFilterRule { Category = category, Level = level });
        return builder;
    }
}
