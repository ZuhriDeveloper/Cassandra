using Cassandra.Domain.Common;

namespace Cassandra.Domain.Warna.Events;

public record WarnaDeactivated(
    WarnaId WarnaId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
