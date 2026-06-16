using Cassandra.Domain.Common;

namespace Cassandra.Domain.DiscountCash.Events;

public record DiscountCashCreated(
    DiscountCashId DiscountCashId,
    Guid DealerId,
    Guid TipeMotorId,
    decimal DirectDiscount,
    decimal ChannelDiscount,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
