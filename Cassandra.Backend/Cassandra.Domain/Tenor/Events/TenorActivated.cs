using Cassandra.Domain.Common;

namespace Cassandra.Domain.Tenor.Events;

public record TenorActivated(
    TenorId TenorId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
