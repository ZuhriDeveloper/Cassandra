using Cassandra.Application.Contracts.CashOutTransaction;
using Cassandra.Application.DTOs.CashOutTransaction;

namespace Cassandra.Application.Queries.CashOutTransaction;

public record GetCashOutTransactionsQuery;
public record GetCashOutTransactionByIdQuery(Guid Id);

public class GetCashOutTransactionsQueryHandler(ICashOutTransactionQueryRepository queryRepository)
{
    public async Task<IReadOnlyList<CashOutTransactionDto>> HandleAsync(GetCashOutTransactionsQuery query, CancellationToken ct = default) =>
        await queryRepository.GetAllAsync(ct);

    public async Task<CashOutTransactionDto?> HandleByIdAsync(GetCashOutTransactionByIdQuery query, CancellationToken ct = default) =>
        await queryRepository.GetByIdAsync(query.Id, ct);
}
