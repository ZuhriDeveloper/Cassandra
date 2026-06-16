using Cassandra.Domain.Common;

namespace Cassandra.Domain.CabangLeasing.Events;

public record CabangLeasingUpdated(
    CabangLeasingId CabangLeasingId,
    string Name,
    string? Phone,
    string? Fax,
    string? Contact,
    Guid GlobalLeasingId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
