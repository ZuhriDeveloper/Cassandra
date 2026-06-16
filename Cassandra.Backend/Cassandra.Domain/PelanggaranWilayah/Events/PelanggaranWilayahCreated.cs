using Cassandra.Domain.Common;

namespace Cassandra.Domain.PelanggaranWilayah.Events;

public record PelanggaranWilayahCreated(
    PelanggaranWilayahId PelanggaranWilayahId,
    Guid DealerId,
    string AreaCode,
    decimal ExtraFee,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
