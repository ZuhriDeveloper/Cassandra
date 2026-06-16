using Cassandra.Domain.Common;

namespace Cassandra.Domain.Discount.Events;

public record DiscountActivated(
    DiscountId DiscountId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
