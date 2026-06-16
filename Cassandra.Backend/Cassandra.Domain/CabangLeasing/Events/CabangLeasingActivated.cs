using Cassandra.Domain.Common;

namespace Cassandra.Domain.CabangLeasing.Events;

public record CabangLeasingActivated(
    CabangLeasingId CabangLeasingId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
