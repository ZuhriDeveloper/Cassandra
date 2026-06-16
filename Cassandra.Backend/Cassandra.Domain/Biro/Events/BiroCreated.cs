using Cassandra.Domain.Common;

namespace Cassandra.Domain.Biro.Events;

public record BiroCreated(
    BiroId BiroId,
    Guid DealerId,
    string Code,
    string Name,
    string? Phone,
    string? Fax,
    string? Pic,
    string? Address,
    decimal PphRate,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
