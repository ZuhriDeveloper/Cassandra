using Cassandra.Domain.Common;

namespace Cassandra.Domain.Tenor.Events;

public record TenorCreated(
    TenorId TenorId,
    Guid DealerId,
    string Code,
    string Name,
    int Months,
    Guid GrupTenorId,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
