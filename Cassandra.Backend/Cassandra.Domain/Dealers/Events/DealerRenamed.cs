using Cassandra.Domain.Common;

namespace Cassandra.Domain.Dealers.Events;

public record DealerRenamed(
    DealerId DealerId,
    string Name,
    string RenamedBy,
    DateTime OccurredAt) : IDomainEvent;
