namespace Cassandra.Application.Commands.Warna.SetWarnaStatus;

public record SetWarnaStatusCommand(Guid Id, bool IsActive, string UpdatedBy);
