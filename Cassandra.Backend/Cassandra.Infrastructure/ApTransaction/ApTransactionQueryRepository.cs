using Cassandra.Application.Contracts.ApTransaction;
using Cassandra.Application.DTOs.ApTransaction;
using Cassandra.Application.DTOs.ArTransaction;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.ApTransaction;

public class ApTransactionQueryRepository(AppDbContext context) : IApTransactionQueryRepository
{
    public async Task<IReadOnlyList<ApTransactionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var models = await context.ApTransactions
            .Include(x => x.Payments)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return models.Select(ToDto).ToList();
    }

    public async Task<ApTransactionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var model = await context.ApTransactions
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return model is null ? null : ToDto(model);
    }

    public async Task<IReadOnlyList<ApTransactionDto>> GetByStnkIdAsync(Guid stnkId, CancellationToken ct = default)
    {
        var models = await context.ApTransactions
            .Include(x => x.Payments)
            .Where(x => x.StnkId == stnkId)
            .ToListAsync(ct);

        return models.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<ArPaymentEntryDto>> GetAllPaymentEntriesAsync(string? transactionTypeFilter, CancellationToken ct = default)
    {
        var query = context.ApTransactions
            .Include(x => x.Payments)
            .AsQueryable();

        if (transactionTypeFilter is not null)
            query = query.Where(x => x.TransactionType == transactionTypeFilter);

        var models = await query.ToListAsync(ct);

        return models
            .SelectMany(ap => ap.Payments.Select(p => new ArPaymentEntryDto(
                p.PaymentNo, p.Amount, p.PaymentDate, p.PaymentMethod, p.FInvoiceId, p.CreatedBy)))
            .ToList();
    }

    private static ApTransactionDto ToDto(Persistence.Projections.ApTransactionReadModel m) =>
        new(m.Id, m.TransactionType, m.StnkId, m.NoRangka,
            m.TotalAmount, m.RemainingAmount, m.IsClosed, m.CreatedBy, m.CreatedAt,
            m.Payments.Select(p => new ArPaymentEntryDto(
                p.PaymentNo, p.Amount, p.PaymentDate, p.PaymentMethod, p.FInvoiceId, p.CreatedBy))
                .ToList());
}
