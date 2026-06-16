using Cassandra.Domain.Common;

namespace Cassandra.Domain.Samsat.Events;

public record SamsatActivated(
    SamsatId SamsatId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
