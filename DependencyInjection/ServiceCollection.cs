namespace MiniLogging.DependencyInjection;

/// <summary>Describes how to create a service: a singleton factory or a type to construct.</summary>
public sealed class ServiceDescriptor
{
    public required Type ServiceType { get; init; }
    public Func<ServiceProvider, object>? Factory { get; init; }
    public Type? ImplementationType { get; init; }
    public bool Singleton { get; init; }
}

/// <summary>A tiny stand-in for IServiceCollection: a list of registrations.</summary>
public sealed class ServiceCollection
{
    internal List<ServiceDescriptor> Descriptors { get; } = new();

    public ServiceCollection AddSingleton(Type serviceType, Func<ServiceProvider, object> factory)
    {
        Descriptors.Add(new ServiceDescriptor
        {
            ServiceType = serviceType,
            Factory = factory,
            Singleton = true
        });
        return this;
    }

    public ServiceCollection AddTransient(Type serviceType, Type? implementationType = null)
    {
        Descriptors.Add(new ServiceDescriptor
        {
            ServiceType = serviceType,
            ImplementationType = implementationType ?? serviceType,
            Singleton = false
        });
        return this;
    }

    public ServiceProvider BuildServiceProvider() => new(this);
}
