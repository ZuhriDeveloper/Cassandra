using Cassandra.Domain.Common;

namespace Cassandra.Domain.PelanggaranWilayah.Events;

public record PelanggaranWilayahDeactivated(
    PelanggaranWilayahId PelanggaranWilayahId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
