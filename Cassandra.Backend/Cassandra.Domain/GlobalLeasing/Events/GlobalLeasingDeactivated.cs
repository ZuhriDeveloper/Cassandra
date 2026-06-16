using Cassandra.Domain.Common;

namespace Cassandra.Domain.GlobalLeasing.Events;

public record GlobalLeasingDeactivated(
    GlobalLeasingId GlobalLeasingId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
