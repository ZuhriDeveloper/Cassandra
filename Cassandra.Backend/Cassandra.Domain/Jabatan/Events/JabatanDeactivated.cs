using Cassandra.Domain.Common;

namespace Cassandra.Domain.Jabatan.Events;

public record JabatanDeactivated(
    JabatanId JabatanId,
    string DeactivatedBy,
    DateTime OccurredAt) : IDomainEvent;
