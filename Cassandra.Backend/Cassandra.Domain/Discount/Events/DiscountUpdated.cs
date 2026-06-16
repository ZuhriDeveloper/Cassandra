using Cassandra.Domain.Common;

namespace Cassandra.Domain.Discount.Events;

public record DiscountUpdated(
    DiscountId DiscountId,
    Guid DaftarHargaLeasingId,
    string Level,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
