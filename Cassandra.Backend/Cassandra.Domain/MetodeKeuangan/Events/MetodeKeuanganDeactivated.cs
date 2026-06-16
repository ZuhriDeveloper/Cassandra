using Cassandra.Domain.Common;

namespace Cassandra.Domain.MetodeKeuangan.Events;

public record MetodeKeuanganDeactivated(
    MetodeKeuanganId MetodeKeuanganId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
