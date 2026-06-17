using Cassandra.Application.Contracts.ApTransaction;
using Cassandra.Application.DTOs.ApTransaction;

namespace Cassandra.Application.Queries.ApTransaction;

public record GetApTransactionsQuery;
public record GetApTransactionByIdQuery(Guid Id);

public class GetApTransactionsQueryHandler(IApTransactionQueryRepository queryRepository)
{
    public async Task<IReadOnlyList<ApTransactionDto>> HandleAsync(GetApTransactionsQuery query, CancellationToken ct = default) =>
        await queryRepository.GetAllAsync(ct);

    public async Task<ApTransactionDto?> HandleByIdAsync(GetApTransactionByIdQuery query, CancellationToken ct = default) =>
        await queryRepository.GetByIdAsync(query.Id, ct);
}
