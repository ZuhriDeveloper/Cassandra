using Cassandra.Domain.Common;

namespace Cassandra.Domain.Biro.Events;

public record BiroActivated(
    BiroId BiroId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
