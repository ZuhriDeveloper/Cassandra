using Cassandra.Domain.Common;

namespace Cassandra.Domain.DiscountCash.Events;

public record DiscountCashUpdated(
    DiscountCashId DiscountCashId,
    decimal DirectDiscount,
    decimal ChannelDiscount,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
