using Cassandra.Domain.Common;

namespace Cassandra.Domain.Kelengkapan.Events;

public record KelengkapanUpdated(
    KelengkapanId KelengkapanId,
    string Name,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
