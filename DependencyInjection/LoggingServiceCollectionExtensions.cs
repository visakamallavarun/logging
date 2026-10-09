namespace MiniLogging.DependencyInjection;

/// <summary>
/// The entry point from the attachment: AddLogging builds the logging
/// infrastructure, runs your configuration callback, then registers the
/// ILoggerFactory singleton the container uses to produce ILogger&lt;T&gt;.
/// </summary>
public static class LoggingServiceCollectionExtensions
{
    public static ServiceCollection AddLogging(
        this ServiceCollection services,
        Action<ILoggingBuilder> configure)
    {
        var builder = new LoggingBuilder();
        configure(builder);

        var factory = new LoggerFactory(builder.Providers, builder.FilterOptions);
        services.AddSingleton(typeof(ILoggerFactory), _ => factory);

        return services;
    }
}
