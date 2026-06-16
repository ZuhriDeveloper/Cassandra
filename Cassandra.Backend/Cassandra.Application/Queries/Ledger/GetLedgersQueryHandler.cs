using Cassandra.Application.Contracts.Ledger;
using Cassandra.Application.DTOs.Ledger;

namespace Cassandra.Application.Queries.Ledger;

public record GetLedgersQuery;

public record GetLedgerByIdQuery(Guid Id);

public class GetLedgersQueryHandler(ILedgerQueryRepository repository)
{
    public Task<IReadOnlyList<LedgerDto>> HandleAsync(GetLedgersQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<LedgerDto?> HandleByIdAsync(GetLedgerByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
