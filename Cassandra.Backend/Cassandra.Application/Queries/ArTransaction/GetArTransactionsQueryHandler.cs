using Cassandra.Application.Contracts.ArTransaction;
using Cassandra.Application.DTOs.ArTransaction;

namespace Cassandra.Application.Queries.ArTransaction;

public record GetArTransactionsQuery;
public record GetArTransactionByIdQuery(Guid Id);

public class GetArTransactionsQueryHandler(IArTransactionQueryRepository queryRepository)
{
    public async Task<IReadOnlyList<ArTransactionDto>> HandleAsync(GetArTransactionsQuery query, CancellationToken ct = default) =>
        await queryRepository.GetAllAsync(ct);

    public async Task<ArTransactionDto?> HandleByIdAsync(GetArTransactionByIdQuery query, CancellationToken ct = default) =>
        await queryRepository.GetByIdAsync(query.Id, ct);
}
