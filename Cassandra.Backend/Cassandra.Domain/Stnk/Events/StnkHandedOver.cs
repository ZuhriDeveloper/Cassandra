using Cassandra.Domain.Common;

namespace Cassandra.Domain.Stnk.Events;

public record StnkHandedOver(
    StnkId   StnkId,
    DateOnly HandoverDate,
    string   StnkReceiver,
    string   UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
