using Cassandra.Domain.Common;

namespace Cassandra.Domain.Mediator.Events;

public record MediatorUpdated(
    MediatorId MediatorId,
    string     Name,
    Guid       KaryawanId,
    string     Address,
    string     UpdatedBy,
    DateTime   OccurredAt) : IDomainEvent;
