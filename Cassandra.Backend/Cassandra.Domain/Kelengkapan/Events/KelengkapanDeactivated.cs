using Cassandra.Domain.Common;

namespace Cassandra.Domain.Kelengkapan.Events;

public record KelengkapanDeactivated(
    KelengkapanId KelengkapanId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
