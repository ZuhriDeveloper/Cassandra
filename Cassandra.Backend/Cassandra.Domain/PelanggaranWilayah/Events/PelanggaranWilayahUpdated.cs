using Cassandra.Domain.Common;

namespace Cassandra.Domain.PelanggaranWilayah.Events;

public record PelanggaranWilayahUpdated(
    PelanggaranWilayahId PelanggaranWilayahId,
    decimal ExtraFee,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
