namespace Cassandra.Application.DTOs.Ledger;

public record LedgerDto(
    Guid Id,
    string Name,
    bool IsActive);
