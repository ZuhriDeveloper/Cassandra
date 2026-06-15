namespace Cassandra.Infrastructure.Persistence.EventStore;

/// <summary>
/// Append-only event store row. Every event is tagged with the <see cref="DealerId"/> it
/// belongs to (the tenant boundary); a dealer aggregate self-owns its events under its own id.
/// </summary>
public class StoredEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid DealerId { get; init; }
    public string AggregateType { get; init; } = default!;
    public Guid AggregateId { get; init; }
    public long Version { get; init; }
    public string EventType { get; init; } = default!;
    public string EventData { get; init; } = default!;
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
