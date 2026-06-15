using Cassandra.Domain.Common;

namespace Cassandra.Domain.Mediator.Events;

public record MediatorCreated(
    MediatorId MediatorId,
    Guid       DealerId,
    string     Name,
    Guid       KaryawanId,
    string     Address,
    decimal    Limit,
    string     CreatedBy,
    DateTime   OccurredAt) : IDomainEvent;
