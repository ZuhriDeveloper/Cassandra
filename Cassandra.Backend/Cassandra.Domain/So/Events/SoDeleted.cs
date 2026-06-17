using Cassandra.Domain.Common;

namespace Cassandra.Domain.So.Events;

public record SoDeleted(
    SoId Id,
    string DeletedBy,
    DateTime OccurredAt) : IDomainEvent;
