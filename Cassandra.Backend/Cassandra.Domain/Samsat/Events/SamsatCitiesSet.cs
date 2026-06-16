using Cassandra.Domain.Common;

namespace Cassandra.Domain.Samsat.Events;

public record SamsatCitiesSet(
    SamsatId SamsatId,
    IReadOnlyList<string> Cities,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
