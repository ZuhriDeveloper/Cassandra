namespace Cassandra.Application.Commands.Mediator.SetMediatorStatus;

public record SetMediatorStatusCommand(Guid Id, bool IsActive, string ChangedBy);
