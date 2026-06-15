namespace Cassandra.Application.Commands.Kios.SetKiosStatus;

public record SetKiosStatusCommand(Guid Id, bool IsActive, string ChangedBy);
