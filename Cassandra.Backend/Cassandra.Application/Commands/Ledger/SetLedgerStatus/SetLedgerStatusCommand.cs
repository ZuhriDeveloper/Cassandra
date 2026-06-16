namespace Cassandra.Application.Commands.Ledger.SetLedgerStatus;

public record SetLedgerStatusCommand(Guid Id, bool IsActive, string UpdatedBy);
