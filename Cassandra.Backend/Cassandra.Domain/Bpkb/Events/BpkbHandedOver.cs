using Cassandra.Domain.Common;

namespace Cassandra.Domain.Bpkb.Events;

public record BpkbHandedOver(
    BpkbId   BpkbId,
    DateOnly HandoverDate,
    string   Receiver,
    string   UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
