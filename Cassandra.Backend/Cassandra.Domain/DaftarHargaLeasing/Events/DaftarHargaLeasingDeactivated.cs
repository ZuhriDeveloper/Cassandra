using Cassandra.Domain.Common;

namespace Cassandra.Domain.DaftarHargaLeasing.Events;

public record DaftarHargaLeasingDeactivated(
    DaftarHargaLeasingId DaftarHargaLeasingId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
