using Cassandra.Domain.Common;

namespace Cassandra.Domain.So.Events;

public record SoCreated(
    SoId Id,
    Guid DealerId,
    string SoNumber,
    DateOnly SoDate,
    DateOnly DueDate,
    string PaymentType,
    Guid MetodeKeuanganId,
    int QtyUnit,
    decimal Total,
    decimal Subsidi,
    decimal CashDiscount,
    decimal PPn,
    decimal TotalAmount,
    decimal Df,
    IReadOnlyList<SoItemValue> Items,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
