using Cassandra.Domain.Common;

namespace Cassandra.Domain.Samsat.Events;

public record SamsatUpdated(
    SamsatId SamsatId,
    string Name,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
