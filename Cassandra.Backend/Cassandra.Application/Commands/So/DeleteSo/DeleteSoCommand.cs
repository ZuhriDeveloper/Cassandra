namespace Cassandra.Application.Commands.So.DeleteSo;

public record DeleteSoCommand(Guid SoId, string DeletedBy);
