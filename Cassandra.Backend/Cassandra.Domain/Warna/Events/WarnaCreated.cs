using Cassandra.Domain.Common;

namespace Cassandra.Domain.Warna.Events;

public record WarnaCreated(
    WarnaId WarnaId,
    Guid DealerId,
    string Code,
    string Name,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
