using Cassandra.Domain.Common;

namespace Cassandra.Domain.Bpkb.Events;

public record BpkbCreated(
    BpkbId   BpkbId,
    Guid     DealerId,
    Guid     RegistrasiPenjualanId,
    Guid     StnkId,
    DateOnly RequestDate,
    string   CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
