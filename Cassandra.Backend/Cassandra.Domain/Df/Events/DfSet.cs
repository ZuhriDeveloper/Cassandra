using Cassandra.Domain.Common;

namespace Cassandra.Domain.Df.Events;

public record DfSet(
    DfId DfId,
    Guid DealerId,
    decimal Discount,
    decimal Interest,
    DateOnly StartDate,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
