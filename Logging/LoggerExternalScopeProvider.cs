namespace MiniLogging;

/// <summary>
/// Tracks the current chain of scopes using an async-local stack, and lets a
/// provider walk them (outermost first) when formatting a message.
/// </summary>
public sealed class LoggerExternalScopeProvider
{
    private readonly AsyncLocal<Scope?> _current = new();

    public IDisposable Push(object? state)
    {
        var parent = _current.Value;
        var scope = new Scope(this, state, parent);
        _current.Value = scope;
        return scope;
    }

    public void ForEachScope<TState>(Action<object?, TState> callback, TState state)
    {
        void Report(Scope? scope)
        {
            if (scope == null)
                return;

            Report(scope.Parent);
            callback(scope.State, state);
        }

        Report(_current.Value);
    }

    private sealed class Scope : IDisposable
    {
        private readonly LoggerExternalScopeProvider _provider;
        private bool _disposed;

        public Scope(LoggerExternalScopeProvider provider, object? state, Scope? parent)
        {
            _provider = provider;
            State = state;
            Parent = parent;
        }

        public object? State { get; }
        public Scope? Parent { get; }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _provider._current.Value = Parent;
        }
    }
}
