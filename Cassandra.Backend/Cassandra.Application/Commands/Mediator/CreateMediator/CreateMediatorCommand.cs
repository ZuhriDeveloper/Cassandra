namespace Cassandra.Application.Commands.Mediator.CreateMediator;

public record CreateMediatorCommand(
    string  Name,
    Guid    KaryawanId,
    string  Address,
    decimal Limit,
    string  CreatedBy);
