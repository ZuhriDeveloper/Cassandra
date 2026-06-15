using Cassandra.Domain.Common;

namespace Cassandra.Domain.Mediator.Events;

public record MediatorActivated(
    MediatorId MediatorId,
    string     ActivatedBy,
    DateTime   OccurredAt) : IDomainEvent;
