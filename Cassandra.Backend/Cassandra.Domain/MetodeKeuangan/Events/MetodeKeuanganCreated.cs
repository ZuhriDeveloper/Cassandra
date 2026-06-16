using Cassandra.Domain.Common;

namespace Cassandra.Domain.MetodeKeuangan.Events;

public record MetodeKeuanganCreated(
    MetodeKeuanganId MetodeKeuanganId,
    Guid DealerId,
    string Code,
    string Name,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
