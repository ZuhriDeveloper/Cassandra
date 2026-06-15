using Cassandra.Application.DTOs.Dealers;

namespace Cassandra.Application.Contracts.Dealers;

/// <summary>
/// Read side for the dealer registry. The registry is platform-level (visible to SuperAdmin
/// across all dealers), so implementations carry no per-dealer query filter.
/// </summary>
public interface IDealerQueryRepository
{
    Task<DealerDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<DealerDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
}
