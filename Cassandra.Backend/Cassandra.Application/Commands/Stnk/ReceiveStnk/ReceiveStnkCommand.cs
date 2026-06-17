namespace Cassandra.Application.Commands.Stnk.ReceiveStnk;

public record ReceiveStnkCommand(
    Guid     StnkId,
    DateOnly ReceiveDate,
    string   PlateNumber,
    Guid     BiroId,
    string   StnkNumber,
    decimal  StnkCost,
    decimal  NoticeCost,
    decimal  ProgressiveCost,
    string   InvoiceNumber,
    string?  Region,
    decimal  BbnCost,
    decimal  PnbpCost,
    decimal  AdminCost,
    decimal  OtherCost,
    decimal  ServiceCost,
    decimal  PphCost,
    bool     IsInvoiceValid,
    string   UpdatedBy);
