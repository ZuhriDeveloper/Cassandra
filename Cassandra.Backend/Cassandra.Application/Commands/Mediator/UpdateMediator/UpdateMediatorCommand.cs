namespace Cassandra.Application.Commands.Mediator.UpdateMediator;

public record UpdateMediatorCommand(
    Guid    Id,
    string  Name,
    Guid    KaryawanId,
    string  Address,
    string  UpdatedBy);
