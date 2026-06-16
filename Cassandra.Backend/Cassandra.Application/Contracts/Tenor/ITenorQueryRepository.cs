using Cassandra.Application.DTOs.Tenor;

namespace Cassandra.Application.Contracts.Tenor;

public interface ITenorQueryRepository
{
    Task<TenorDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TenorDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
