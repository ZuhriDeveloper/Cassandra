using Cassandra.Domain.Common;

namespace Cassandra.Domain.Stnk.Events;

public record StnkReceived(
    StnkId   StnkId,
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
    string   UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
