namespace Cassandra.Application.Commands.Ledger.UpdateLedger;

public record UpdateLedgerCommand(Guid Id, string Name, string UpdatedBy);
