using Cassandra.Application.DTOs.SoRetur;

namespace Cassandra.Application.Contracts.SoRetur;

public interface ISoReturQueryRepository
{
    Task<IReadOnlyList<SoReturDto>> GetAllAsync(CancellationToken ct = default);
    Task<SoReturDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SoReturDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ReturNumberExistsAsync(string returNumber, CancellationToken ct = default);
}
