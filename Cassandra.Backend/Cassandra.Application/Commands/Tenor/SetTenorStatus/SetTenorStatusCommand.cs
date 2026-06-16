namespace Cassandra.Application.Commands.Tenor.SetTenorStatus;

public record SetTenorStatusCommand(Guid Id, bool IsActive, string ActionBy);
