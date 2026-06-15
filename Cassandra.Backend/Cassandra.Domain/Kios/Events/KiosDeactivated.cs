using Cassandra.Domain.Common;

namespace Cassandra.Domain.Kios.Events;

public record KiosDeactivated(
    KiosId   KiosId,
    string   DeactivatedBy,
    DateTime OccurredAt) : IDomainEvent;
