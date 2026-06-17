using Cassandra.Application.DTOs.So;

namespace Cassandra.Application.Contracts.So;

public interface ISoQueryRepository
{
    Task<IReadOnlyList<SoDto>> GetAllAsync(CancellationToken ct = default);
    Task<SoDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SoDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<bool> SoNumberExistsAsync(string soNumber, CancellationToken ct = default);
    Task<bool> IsSoAktifAsync(Guid soId, CancellationToken ct = default);
}
