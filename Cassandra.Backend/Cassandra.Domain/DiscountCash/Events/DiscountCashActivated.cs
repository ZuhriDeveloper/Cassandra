using Cassandra.Domain.Common;

namespace Cassandra.Domain.DiscountCash.Events;

public record DiscountCashActivated(
    DiscountCashId DiscountCashId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
