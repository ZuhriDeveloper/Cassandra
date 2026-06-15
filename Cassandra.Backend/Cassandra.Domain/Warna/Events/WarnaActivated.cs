using Cassandra.Domain.Common;

namespace Cassandra.Domain.Warna.Events;

public record WarnaActivated(
    WarnaId WarnaId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
