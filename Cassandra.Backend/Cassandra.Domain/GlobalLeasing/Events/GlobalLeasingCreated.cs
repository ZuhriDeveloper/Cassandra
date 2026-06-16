using Cassandra.Domain.Common;

namespace Cassandra.Domain.GlobalLeasing.Events;

public record GlobalLeasingCreated(
    GlobalLeasingId GlobalLeasingId,
    Guid DealerId,
    string Code,
    string Name,
    string Phone,
    string? Fax,
    string Contact,
    string Address,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
