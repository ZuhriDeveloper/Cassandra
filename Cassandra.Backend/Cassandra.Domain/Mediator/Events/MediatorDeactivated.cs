using Cassandra.Domain.Common;

namespace Cassandra.Domain.Mediator.Events;

public record MediatorDeactivated(
    MediatorId MediatorId,
    string     DeactivatedBy,
    DateTime   OccurredAt) : IDomainEvent;
