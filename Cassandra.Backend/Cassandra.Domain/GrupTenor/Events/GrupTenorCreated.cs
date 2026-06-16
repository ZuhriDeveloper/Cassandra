using Cassandra.Domain.Common;

namespace Cassandra.Domain.GrupTenor.Events;

public record GrupTenorCreated(
    GrupTenorId GrupTenorId,
    Guid DealerId,
    string Code,
    string Name,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
