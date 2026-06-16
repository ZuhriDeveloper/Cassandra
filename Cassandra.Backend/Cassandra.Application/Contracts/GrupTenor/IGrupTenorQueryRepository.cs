using Cassandra.Application.DTOs.GrupTenor;

namespace Cassandra.Application.Contracts.GrupTenor;

public interface IGrupTenorQueryRepository
{
    Task<GrupTenorDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<GrupTenorDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
