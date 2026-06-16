using Cassandra.Application.DTOs.Biro;

namespace Cassandra.Application.Contracts.Biro;

public interface IBiroQueryRepository
{
    Task<BiroDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<BiroDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
