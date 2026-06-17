using Cassandra.Domain.Common;

namespace Cassandra.Domain.Stock.Events;

public record StockMoved(
    StockId Id,
    Guid NewKiosId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
