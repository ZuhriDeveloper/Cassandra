using Cassandra.Domain.Common;

namespace Cassandra.Domain.Stock.Events;

public record StockStatusChanged(
    StockId Id,
    string Status,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
