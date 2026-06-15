using Cassandra.Domain.Common;

namespace Cassandra.Domain.Kios.Events;

public record KiosCreated(
    KiosId   KiosId,
    Guid     DealerId,
    string   Code,
    string   Name,
    string   Phone,
    string?  Fax,
    string   Address,
    Guid     PicKaryawanId,
    decimal  Limit,
    string   CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
