using Cassandra.Application.DTOs.Discount;

namespace Cassandra.Application.Contracts.Discount;

public interface IDiscountQueryRepository
{
    Task<DiscountDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<DiscountDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid daftarHargaLeasingId, string level, CancellationToken ct = default);
    Task<bool> ExistsExcludingAsync(Guid daftarHargaLeasingId, string level, Guid excludeId, CancellationToken ct = default);
}
