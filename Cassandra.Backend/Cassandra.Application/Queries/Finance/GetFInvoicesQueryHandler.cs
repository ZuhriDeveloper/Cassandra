using Cassandra.Application.Contracts.ApTransaction;
using Cassandra.Application.Contracts.ArTransaction;
using Cassandra.Application.DTOs.Finance;

namespace Cassandra.Application.Queries.Finance;

public record GetFInvoicesQuery(string? TransactionTypeFilter = null);

public class GetFInvoicesQueryHandler(
    IArTransactionQueryRepository arQueryRepository,
    IApTransactionQueryRepository apQueryRepository)
{
    public async Task<IReadOnlyList<FInvoiceDto>> HandleAsync(GetFInvoicesQuery query, CancellationToken ct = default)
    {
        var arEntries = await arQueryRepository.GetAllPaymentEntriesAsync(query.TransactionTypeFilter, ct);
        var apEntries = await apQueryRepository.GetAllPaymentEntriesAsync(query.TransactionTypeFilter, ct);

        // Get AR transactions for mapping FInvoiceId -> ReferenceNumber / TransactionType
        var arTransactions = await arQueryRepository.GetAllAsync(ct);
        var apTransactions = await apQueryRepository.GetAllAsync(ct);

        var result = new List<FInvoiceDto>();

        foreach (var ar in arTransactions)
        {
            if (query.TransactionTypeFilter is not null && ar.TransactionType != query.TransactionTypeFilter)
                continue;

            foreach (var p in ar.Payments)
            {
                if (string.IsNullOrEmpty(p.FInvoiceId)) continue;
                result.Add(new FInvoiceDto(
                    p.FInvoiceId,
                    ar.TransactionType,
                    ar.ReferenceNumber,
                    p.Amount,
                    p.PaymentDate,
                    p.PaymentMethod,
                    p.CreatedBy));
            }
        }

        foreach (var ap in apTransactions)
        {
            if (query.TransactionTypeFilter is not null && ap.TransactionType != query.TransactionTypeFilter)
                continue;

            foreach (var p in ap.Payments)
            {
                if (string.IsNullOrEmpty(p.FInvoiceId)) continue;
                result.Add(new FInvoiceDto(
                    p.FInvoiceId,
                    ap.TransactionType,
                    ap.NoRangka,
                    p.Amount,
                    p.PaymentDate,
                    p.PaymentMethod,
                    p.CreatedBy));
            }
        }

        return result.OrderByDescending(x => x.PaymentDate).ToList();
    }
}
