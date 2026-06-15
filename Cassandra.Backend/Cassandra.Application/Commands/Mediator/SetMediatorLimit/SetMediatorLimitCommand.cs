namespace Cassandra.Application.Commands.Mediator.SetMediatorLimit;

public record SetMediatorLimitCommand(Guid Id, decimal Limit, string SetBy);
