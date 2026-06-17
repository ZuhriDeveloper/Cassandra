using Cassandra.Domain.Common;

namespace Cassandra.Domain.So.Events;

public record SoStatusChanged(
    SoId Id,
    string Status,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
