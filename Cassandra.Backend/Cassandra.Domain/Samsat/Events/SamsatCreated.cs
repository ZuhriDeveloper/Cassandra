using Cassandra.Domain.Common;

namespace Cassandra.Domain.Samsat.Events;

public record SamsatCreated(
    SamsatId SamsatId,
    Guid DealerId,
    string Name,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
