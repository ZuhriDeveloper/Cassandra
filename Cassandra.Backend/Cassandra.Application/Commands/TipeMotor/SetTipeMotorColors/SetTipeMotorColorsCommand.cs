namespace Cassandra.Application.Commands.TipeMotor.SetTipeMotorColors;

public record SetTipeMotorColorsCommand(Guid Id, List<Guid> WarnaIds, string UpdatedBy);
