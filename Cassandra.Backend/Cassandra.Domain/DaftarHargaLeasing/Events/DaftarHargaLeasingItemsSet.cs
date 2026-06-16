using Cassandra.Domain.Common;

namespace Cassandra.Domain.DaftarHargaLeasing.Events;

public record DaftarHargaLeasingItemsSet(
    DaftarHargaLeasingId DaftarHargaLeasingId,
    IReadOnlyList<DaftarHargaLeasingItem> Items,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
