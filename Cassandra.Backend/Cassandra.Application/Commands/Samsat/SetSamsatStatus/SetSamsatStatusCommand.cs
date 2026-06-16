namespace Cassandra.Application.Commands.Samsat.SetSamsatStatus;

public record SetSamsatStatusCommand(Guid Id, bool IsActive, string UpdatedBy);
