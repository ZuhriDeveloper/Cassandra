using Cassandra.Domain.Common;

namespace Cassandra.Domain.DaftarHargaLeasing.Events;

public record DaftarHargaLeasingActivated(
    DaftarHargaLeasingId DaftarHargaLeasingId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
