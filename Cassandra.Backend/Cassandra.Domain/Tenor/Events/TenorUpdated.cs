using Cassandra.Domain.Common;

namespace Cassandra.Domain.Tenor.Events;

public record TenorUpdated(
    TenorId TenorId,
    string Name,
    int Months,
    Guid GrupTenorId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
