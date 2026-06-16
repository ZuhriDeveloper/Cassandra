using Cassandra.Application.DTOs.DiscountCash;

namespace Cassandra.Application.Contracts.DiscountCash;

public interface IDiscountCashQueryRepository
{
    Task<DiscountCashDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<DiscountCashDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> TipeMotorIdExistsAsync(Guid tipeMotorId, CancellationToken ct = default);
}
