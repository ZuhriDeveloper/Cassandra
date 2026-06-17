using Cassandra.Domain.Common;

namespace Cassandra.Domain.SoRetur.Events;

public record SoReturCreated(
    SoReturId Id,
    Guid DealerId,
    string ReturNumber,
    Guid SoId,
    DateOnly ReturDate,
    string Reason,
    decimal Total,
    decimal PPn,
    decimal TotalAmount,
    IReadOnlyList<SoReturItemValue> Items,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
