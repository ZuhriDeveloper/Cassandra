using Cassandra.Domain.Common;

namespace Cassandra.Domain.Jabatan.Events;

public record JabatanUpdated(
    JabatanId JabatanId,
    string Name,
    string Description,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
