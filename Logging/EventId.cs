namespace MiniLogging;

/// <summary>Identifies a log event. An int converts implicitly to an EventId.</summary>
public readonly struct EventId
{
    public int Id { get; }
    public string? Name { get; }

    public EventId(int id, string? name = null)
    {
        Id = id;
        Name = name;
    }

    public static implicit operator EventId(int id) => new(id);

    public override string ToString() => Name ?? Id.ToString();
}
