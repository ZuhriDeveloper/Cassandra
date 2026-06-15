using Cassandra.Domain.Common;

namespace Cassandra.Domain.Dealers.Events;

public record DealerActivated(
    DealerId DealerId,
    string ActivatedBy,
    DateTime OccurredAt) : IDomainEvent;
