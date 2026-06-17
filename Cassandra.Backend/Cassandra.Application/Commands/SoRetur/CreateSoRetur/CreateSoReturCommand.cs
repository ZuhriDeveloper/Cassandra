namespace Cassandra.Application.Commands.SoRetur.CreateSoRetur;

public record CreateSoReturItemRequest(Guid TipeMotorId, Guid WarnaId, int Qty, decimal NettPrice);

public record CreateSoReturCommand(
    string ReturNumber,
    Guid SoId,
    DateOnly ReturDate,
    string Reason,
    List<CreateSoReturItemRequest> Items,
    string CreatedBy);
