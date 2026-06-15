using Cassandra.Domain.Common;

namespace Cassandra.Domain.GrupTipeMotor.Events;

public record GrupTipeMotorCreated(
    GrupTipeMotorId GrupTipeMotorId,
    Guid DealerId,
    string Code,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
