using Cassandra.Domain.Common;

namespace Cassandra.Domain.Kelengkapan.Events;

public record KelengkapanActivated(
    KelengkapanId KelengkapanId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
