using Cassandra.Domain.Common;

namespace Cassandra.Domain.Biro.Events;

public record BiroDeactivated(
    BiroId BiroId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
