using Cassandra.Domain.Common;

namespace Cassandra.Domain.TipeMotor.Events;

public record TipeMotorDeactivated(
    TipeMotorId TipeMotorId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
