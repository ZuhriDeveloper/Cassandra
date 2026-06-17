namespace Cassandra.Application.Commands.So.ChangeSoStatus;

public record ChangeSoStatusCommand(Guid SoId, string Status, string UpdatedBy);
