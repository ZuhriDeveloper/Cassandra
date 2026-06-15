using Cassandra.Domain.Common;

namespace Cassandra.Domain.Mediator.Events;

public record MediatorLimitSet(
    MediatorId MediatorId,
    decimal    Limit,
    string     SetBy,
    DateTime   OccurredAt) : IDomainEvent;
