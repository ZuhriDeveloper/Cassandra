namespace Cassandra.Application.Commands.GrupTipeMotor.SetGrupTipeMotorStatus;

public record SetGrupTipeMotorStatusCommand(Guid Id, bool IsActive, string UpdatedBy);
