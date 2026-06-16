using Cassandra.Domain.Discount;

namespace Cassandra.Application.Contracts.Discount;

public interface IDiscountRepository
{
    Task<Domain.Discount.Discount?> GetByIdAsync(DiscountId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Discount.Discount discount, CancellationToken ct = default);
}
