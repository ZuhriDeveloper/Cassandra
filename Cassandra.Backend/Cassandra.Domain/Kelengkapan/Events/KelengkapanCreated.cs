using Cassandra.Domain.Common;

namespace Cassandra.Domain.Kelengkapan.Events;

public record KelengkapanCreated(
    KelengkapanId KelengkapanId,
    Guid DealerId,
    string Name,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
