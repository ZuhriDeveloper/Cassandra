namespace Cassandra.Application.Commands.Kelengkapan.SetKelengkapanStatus;

public record SetKelengkapanStatusCommand(Guid Id, bool IsActive, string UpdatedBy);
