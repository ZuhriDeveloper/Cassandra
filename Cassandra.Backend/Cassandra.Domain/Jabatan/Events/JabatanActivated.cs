using Cassandra.Domain.Common;

namespace Cassandra.Domain.Jabatan.Events;

public record JabatanActivated(
    JabatanId JabatanId,
    string ActivatedBy,
    DateTime OccurredAt) : IDomainEvent;
