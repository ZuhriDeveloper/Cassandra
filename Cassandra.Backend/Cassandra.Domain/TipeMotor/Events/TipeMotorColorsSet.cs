using Cassandra.Domain.Common;

namespace Cassandra.Domain.TipeMotor.Events;

public record TipeMotorColorsSet(
    TipeMotorId TipeMotorId,
    IReadOnlyList<Guid> WarnaIds,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
