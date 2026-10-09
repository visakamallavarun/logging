namespace MiniLogging.DependencyInjection;

/// <summary>
/// A minimal container. It resolves registered services, constructs concrete
/// types via their first constructor, and special-cases the open generic
/// ILogger&lt;T&gt; by routing to the ILoggerFactory - just like the real DI does.
/// </summary>
public sealed class ServiceProvider : IDisposable
{
    private readonly List<ServiceDescriptor> _descriptors;
    private readonly Dictionary<Type, object> _singletons = new();
    private readonly List<IDisposable> _disposables = new();

    public ServiceProvider(ServiceCollection services) => _descriptors = services.Descriptors;

    public T GetRequiredService<T>()
        => (T)(GetService(typeof(T))
            ?? throw new InvalidOperationException($"No service registered for {typeof(T)}."));

    public object? GetService(Type serviceType)
    {
        // Open generic: ILogger<TCategory> -> new Logger<TCategory>(ILoggerFactory)
        if (serviceType.IsGenericType
            && serviceType.GetGenericTypeDefinition() == typeof(ILogger<>))
        {
            var factory = (ILoggerFactory)GetService(typeof(ILoggerFactory))!;
            Type loggerType = typeof(Logger<>).MakeGenericType(serviceType.GetGenericArguments());
            return Activator.CreateInstance(loggerType, factory)!;
        }

        ServiceDescriptor? descriptor = null;
        for (int i = _descriptors.Count - 1; i >= 0; i--)
        {
            if (_descriptors[i].ServiceType == serviceType)
            {
                descriptor = _descriptors[i];
                break;
            }
        }

        if (descriptor != null)
        {
            if (descriptor.Singleton && _singletons.TryGetValue(serviceType, out var existing))
                return existing;

            object instance = descriptor.Factory != null
                ? descriptor.Factory(this)
                : Construct(descriptor.ImplementationType!);

            if (descriptor.Singleton)
                _singletons[serviceType] = instance;

            if (instance is IDisposable disposable)
                _disposables.Add(disposable);

            return instance;
        }

        if (!serviceType.IsAbstract && !serviceType.IsInterface)
            return Construct(serviceType);

        return null;
    }

    private object Construct(Type type)
    {
        var ctor = type.GetConstructors()[0];
        var parameters = ctor.GetParameters();
        var args = new object?[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
            args[i] = GetService(parameters[i].ParameterType);

        return ctor.Invoke(args);
    }

    public void Dispose()
    {
        for (int i = _disposables.Count - 1; i >= 0; i--)
            _disposables[i].Dispose();

        _disposables.Clear();
    }
}
