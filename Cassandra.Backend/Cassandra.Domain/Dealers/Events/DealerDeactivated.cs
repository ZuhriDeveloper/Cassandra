using Cassandra.Domain.Common;

namespace Cassandra.Domain.Dealers.Events;

public record DealerDeactivated(
    DealerId DealerId,
    string DeactivatedBy,
    DateTime OccurredAt) : IDomainEvent;
