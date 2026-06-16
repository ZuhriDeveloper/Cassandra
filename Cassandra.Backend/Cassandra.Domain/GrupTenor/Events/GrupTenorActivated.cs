using Cassandra.Domain.Common;

namespace Cassandra.Domain.GrupTenor.Events;

public record GrupTenorActivated(
    GrupTenorId GrupTenorId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
