using Cassandra.Domain.Common;

namespace Cassandra.Domain.GlobalLeasing.Events;

public record GlobalLeasingUpdated(
    GlobalLeasingId GlobalLeasingId,
    string Name,
    string Phone,
    string? Fax,
    string Contact,
    string Address,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
