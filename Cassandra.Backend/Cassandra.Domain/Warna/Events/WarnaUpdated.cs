using Cassandra.Domain.Common;

namespace Cassandra.Domain.Warna.Events;

public record WarnaUpdated(
    WarnaId WarnaId,
    string Name,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
