using Cassandra.Domain.Common;

namespace Cassandra.Domain.Kios.Events;

public record KiosLimitSet(
    KiosId   KiosId,
    decimal  Limit,
    string   SetBy,
    DateTime OccurredAt) : IDomainEvent;
