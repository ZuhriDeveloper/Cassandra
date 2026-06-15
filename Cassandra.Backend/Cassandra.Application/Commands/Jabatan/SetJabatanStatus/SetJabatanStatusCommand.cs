namespace Cassandra.Application.Commands.Jabatan.SetJabatanStatus;

public record SetJabatanStatusCommand(Guid Id, bool IsActive, string ActionBy);
