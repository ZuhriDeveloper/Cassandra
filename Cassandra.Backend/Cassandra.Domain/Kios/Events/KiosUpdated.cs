using Cassandra.Domain.Common;

namespace Cassandra.Domain.Kios.Events;

public record KiosUpdated(
    KiosId   KiosId,
    string   Name,
    string   Phone,
    string?  Fax,
    string   Address,
    Guid     PicKaryawanId,
    string   UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
