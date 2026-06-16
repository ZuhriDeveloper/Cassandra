using Cassandra.Domain.Common;

namespace Cassandra.Domain.PelanggaranWilayah.Events;

public record PelanggaranWilayahActivated(
    PelanggaranWilayahId PelanggaranWilayahId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
