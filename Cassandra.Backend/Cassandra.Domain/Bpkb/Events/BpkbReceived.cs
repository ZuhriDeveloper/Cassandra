using Cassandra.Domain.Common;

namespace Cassandra.Domain.Bpkb.Events;

public record BpkbReceived(
    BpkbId   BpkbId,
    string   BpkbNumber,
    string   BookNumber,
    DateOnly ReceiveDate,
    string   UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
