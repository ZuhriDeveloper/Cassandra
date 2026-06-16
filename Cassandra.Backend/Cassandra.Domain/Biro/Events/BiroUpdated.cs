using Cassandra.Domain.Common;

namespace Cassandra.Domain.Biro.Events;

public record BiroUpdated(
    BiroId BiroId,
    string Name,
    string? Phone,
    string? Fax,
    string? Pic,
    string? Address,
    decimal PphRate,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
