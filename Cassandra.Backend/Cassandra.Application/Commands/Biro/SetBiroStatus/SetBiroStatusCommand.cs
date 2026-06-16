namespace Cassandra.Application.Commands.Biro.SetBiroStatus;

public record SetBiroStatusCommand(Guid Id, bool IsActive, string UpdatedBy);
