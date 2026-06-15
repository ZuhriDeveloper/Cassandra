using Cassandra.Domain.Common;

namespace Cassandra.Domain.Dealers.Events;

public record DealerRegistered(
    DealerId DealerId,
    string Name,
    string Code,
    string RegisteredBy,
    DateTime OccurredAt) : IDomainEvent;
