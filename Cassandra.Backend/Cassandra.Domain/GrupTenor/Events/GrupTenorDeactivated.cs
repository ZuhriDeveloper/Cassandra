using Cassandra.Domain.Common;

namespace Cassandra.Domain.GrupTenor.Events;

public record GrupTenorDeactivated(
    GrupTenorId GrupTenorId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
