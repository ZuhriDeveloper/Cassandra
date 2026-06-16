using Cassandra.Domain.Common;

namespace Cassandra.Domain.MetodeKeuangan.Events;

public record MetodeKeuanganUpdated(
    MetodeKeuanganId MetodeKeuanganId,
    string Name,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
