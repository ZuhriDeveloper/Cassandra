namespace Cassandra.Application.Contracts.Finance;

public interface IFinanceCounter
{
    Task<string> GetNextFInvoiceIdAsync(CancellationToken ct = default);
}
