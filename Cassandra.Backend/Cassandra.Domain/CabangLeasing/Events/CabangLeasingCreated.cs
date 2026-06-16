using Cassandra.Domain.Common;

namespace Cassandra.Domain.CabangLeasing.Events;

public record CabangLeasingCreated(
    CabangLeasingId CabangLeasingId,
    Guid DealerId,
    string Code,
    string Name,
    string? Phone,
    string? Fax,
    string? Contact,
    Guid GlobalLeasingId,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
