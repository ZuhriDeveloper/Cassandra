using Cassandra.Application.DTOs.Kios;

namespace Cassandra.Application.Contracts.Kios;

public interface IKiosQueryRepository
{
    Task<KiosDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<KiosDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
}
