namespace Cassandra.Application.Commands.So.CreateSo;

public record CreateSoItemRequest(Guid TipeMotorId, Guid WarnaId, int Qty, decimal NettPrice);

public record CreateSoCommand(
    string SoNumber,
    DateOnly SoDate,
    DateOnly DueDate,
    string PaymentType,
    Guid MetodeKeuanganId,
    decimal Subsidi,
    decimal CashDiscount,
    decimal Df,
    List<CreateSoItemRequest> Items,
    string CreatedBy);
