using Cassandra.Domain.Common;

namespace Cassandra.Domain.Jabatan.Events;

public record JabatanCreated(
    JabatanId JabatanId,
    Guid DealerId,
    string Name,
    string Description,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
