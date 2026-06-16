using Cassandra.Domain.Common;

namespace Cassandra.Domain.DiscountCash.Events;

public record DiscountCashDeactivated(
    DiscountCashId DiscountCashId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
