using Cassandra.Domain.Common;

namespace Cassandra.Domain.Kios.Events;

public record KiosActivated(
    KiosId   KiosId,
    string   ActivatedBy,
    DateTime OccurredAt) : IDomainEvent;
