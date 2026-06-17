using Cassandra.Application.Contracts.ArTransaction;
using Cassandra.Application.DTOs.ArTransaction;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.ArTransaction;

public class ArTransactionQueryRepository(AppDbContext context) : IArTransactionQueryRepository
{
    public async Task<IReadOnlyList<ArTransactionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var models = await context.ArTransactions
            .Include(x => x.Payments)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return models.Select(ToDto).ToList();
    }

    public async Task<ArTransactionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var model = await context.ArTransactions
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return model is null ? null : ToDto(model);
    }

    public async Task<IReadOnlyList<ArTransactionDto>> GetByReferenceIdAsync(Guid referenceId, CancellationToken ct = default)
    {
        var models = await context.ArTransactions
            .Include(x => x.Payments)
            .Where(x => x.ReferenceId == referenceId)
            .ToListAsync(ct);

        return models.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<ArPaymentEntryDto>> GetAllPaymentEntriesAsync(string? transactionTypeFilter, CancellationToken ct = default)
    {
        var query = context.ArTransactions
            .Include(x => x.Payments)
            .AsQueryable();

        if (transactionTypeFilter is not null)
            query = query.Where(x => x.TransactionType == transactionTypeFilter);

        var models = await query.ToListAsync(ct);

        return models
            .SelectMany(ar => ar.Payments.Select(p => new ArPaymentEntryDto(
                p.PaymentNo, p.Amount, p.PaymentDate, p.PaymentMethod, p.FInvoiceId, p.CreatedBy)))
            .ToList();
    }

    private static ArTransactionDto ToDto(Persistence.Projections.ArTransactionReadModel m) =>
        new(m.Id, m.TransactionType, m.ReferenceId, m.ReferenceNumber,
            m.TotalAmount, m.RemainingAmount, m.IsClosed, m.CreatedBy, m.CreatedAt,
            m.Payments.Select(p => new ArPaymentEntryDto(
                p.PaymentNo, p.Amount, p.PaymentDate, p.PaymentMethod, p.FInvoiceId, p.CreatedBy))
                .ToList());
}
