using Cassandra.Domain.Common;

namespace Cassandra.Domain.GrupTipeMotor.Events;

public record GrupTipeMotorDeactivated(
    GrupTipeMotorId GrupTipeMotorId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
