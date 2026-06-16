using Cassandra.Domain.Common;

namespace Cassandra.Domain.Discount.Events;

public record DiscountCreated(
    DiscountId DiscountId,
    Guid DealerId,
    Guid DaftarHargaLeasingId,
    string Level,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
