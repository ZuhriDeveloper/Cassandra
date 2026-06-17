using Cassandra.Application.DTOs.Mutasi;

namespace Cassandra.Application.Contracts.Mutasi;

public interface IMutasiQueryRepository
{
    Task<IReadOnlyList<MutasiDto>> GetAllAsync(CancellationToken ct = default);
    Task<MutasiDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<MutasiDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<bool> MutasiNumberExistsAsync(string mutasiNumber, CancellationToken ct = default);
}
