namespace Cassandra.Application.Commands.TipeMotor.SetTipeMotorStatus;

public record SetTipeMotorStatusCommand(Guid Id, bool IsActive, string UpdatedBy);
