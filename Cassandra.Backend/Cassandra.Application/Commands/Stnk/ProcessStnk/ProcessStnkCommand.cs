namespace Cassandra.Application.Commands.Stnk.ProcessStnk;

public record ProcessStnkCommand(
    Guid     StnkId,
    DateOnly ProcessDate,
    Guid     BiroId,
    string   InvoiceNumber,
    string   UpdatedBy);
