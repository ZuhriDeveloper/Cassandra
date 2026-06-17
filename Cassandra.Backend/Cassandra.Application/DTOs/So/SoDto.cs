namespace Cassandra.Application.DTOs.So;

public record SoDto(
    Guid Id,
    string SoNumber,
    DateOnly SoDate,
    DateOnly DueDate,
    string PaymentType,
    Guid MetodeKeuanganId,
    int QtyUnit,
    decimal Total,
    decimal Subsidi,
    decimal CashDiscount,
    decimal PPn,
    decimal TotalAmount,
    decimal Df,
    string Status,
    List<SoItemDto>? Items);

public record SoItemDto(Guid TipeMotorId, Guid WarnaId, int Qty, decimal NettPrice);
