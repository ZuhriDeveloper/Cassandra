using Cassandra.Domain.DiscountCash;

namespace Cassandra.Application.Contracts.DiscountCash;

public interface IDiscountCashRepository
{
    Task<Domain.DiscountCash.DiscountCash?> GetByIdAsync(DiscountCashId id, CancellationToken ct = default);
    Task SaveAsync(Domain.DiscountCash.DiscountCash discountCash, CancellationToken ct = default);
}
