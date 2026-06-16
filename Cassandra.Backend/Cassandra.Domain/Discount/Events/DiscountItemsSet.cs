using Cassandra.Domain.Common;

namespace Cassandra.Domain.Discount.Events;

public record DiscountItemsSet(
    DiscountId DiscountId,
    IReadOnlyList<DiscountLineItem> Items,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
