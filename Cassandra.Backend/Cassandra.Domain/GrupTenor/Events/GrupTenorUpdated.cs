using Cassandra.Domain.Common;

namespace Cassandra.Domain.GrupTenor.Events;

public record GrupTenorUpdated(
    GrupTenorId GrupTenorId,
    string Name,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
