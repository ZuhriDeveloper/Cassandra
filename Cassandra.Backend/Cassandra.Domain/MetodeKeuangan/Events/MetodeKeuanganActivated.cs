using Cassandra.Domain.Common;

namespace Cassandra.Domain.MetodeKeuangan.Events;

public record MetodeKeuanganActivated(
    MetodeKeuanganId MetodeKeuanganId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
