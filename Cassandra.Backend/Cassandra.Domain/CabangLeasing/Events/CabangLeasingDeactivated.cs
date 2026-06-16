using Cassandra.Domain.Common;

namespace Cassandra.Domain.CabangLeasing.Events;

public record CabangLeasingDeactivated(
    CabangLeasingId CabangLeasingId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
