namespace Cassandra.Application.DTOs.SoRetur;

public record SoReturDto(
    Guid Id,
    string ReturNumber,
    Guid SoId,
    DateOnly ReturDate,
    string Reason,
    decimal Total,
    decimal PPn,
    decimal TotalAmount,
    List<SoReturItemDto>? Items);

public record SoReturItemDto(Guid TipeMotorId, Guid WarnaId, int Qty, decimal NettPrice);
