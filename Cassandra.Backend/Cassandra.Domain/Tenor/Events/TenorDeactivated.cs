using Cassandra.Domain.Common;

namespace Cassandra.Domain.Tenor.Events;

public record TenorDeactivated(
    TenorId TenorId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
