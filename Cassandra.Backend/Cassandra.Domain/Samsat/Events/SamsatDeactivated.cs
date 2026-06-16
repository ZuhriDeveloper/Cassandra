using Cassandra.Domain.Common;

namespace Cassandra.Domain.Samsat.Events;

public record SamsatDeactivated(
    SamsatId SamsatId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
