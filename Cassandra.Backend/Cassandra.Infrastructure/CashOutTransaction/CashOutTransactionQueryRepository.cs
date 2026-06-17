using Cassandra.Application.Contracts.CashOutTransaction;
using Cassandra.Application.DTOs.CashOutTransaction;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.CashOutTransaction;

public class CashOutTransactionQueryRepository(AppDbContext context) : ICashOutTransactionQueryRepository
{
    public async Task<IReadOnlyList<CashOutTransactionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var models = await context.CashOutTransactions
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return models.Select(ToDto).ToList();
    }

    public async Task<CashOutTransactionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var model = await context.CashOutTransactions
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return model is null ? null : ToDto(model);
    }

    private static CashOutTransactionDto ToDto(Persistence.Projections.CashOutTransactionReadModel m) =>
        new(m.Id, m.TransactionType, m.SoId, m.SoReturId,
            m.Amount, m.DfAmount, m.TotalHariDf, m.PaymentDate,
            m.PaymentMethod, m.FInvoiceId, m.IsClosed, m.CreatedBy, m.CreatedAt);
}
