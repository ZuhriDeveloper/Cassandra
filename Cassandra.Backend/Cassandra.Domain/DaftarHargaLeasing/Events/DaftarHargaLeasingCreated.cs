using Cassandra.Domain.Common;

namespace Cassandra.Domain.DaftarHargaLeasing.Events;

public record DaftarHargaLeasingCreated(
    DaftarHargaLeasingId DaftarHargaLeasingId,
    Guid DealerId,
    string Name,
    Guid GlobalLeasingId,
    Guid GrupTenorId,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
