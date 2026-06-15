namespace Cassandra.Application.Commands.Kios.SetKiosLimit;

public record SetKiosLimitCommand(Guid Id, decimal Limit, string SetBy);
