using Cassandra.Domain.Common;

namespace Cassandra.Domain.TipeMotor.Events;

public record TipeMotorActivated(
    TipeMotorId TipeMotorId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
