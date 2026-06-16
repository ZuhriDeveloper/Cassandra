using Cassandra.Domain.Common;

namespace Cassandra.Domain.Discount.Events;

public record DiscountDeactivated(
    DiscountId DiscountId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
