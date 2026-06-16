using Cassandra.Domain.Common;

namespace Cassandra.Domain.GlobalLeasing.Events;

public record GlobalLeasingActivated(
    GlobalLeasingId GlobalLeasingId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
