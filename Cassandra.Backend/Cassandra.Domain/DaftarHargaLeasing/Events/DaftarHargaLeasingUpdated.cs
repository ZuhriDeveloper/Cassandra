using Cassandra.Domain.Common;

namespace Cassandra.Domain.DaftarHargaLeasing.Events;

public record DaftarHargaLeasingUpdated(
    DaftarHargaLeasingId DaftarHargaLeasingId,
    string Name,
    Guid GlobalLeasingId,
    Guid GrupTenorId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
