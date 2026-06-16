using Cassandra.Application.DTOs.GlobalLeasing;

namespace Cassandra.Application.Contracts.GlobalLeasing;

public interface IGlobalLeasingQueryRepository
{
    Task<GlobalLeasingDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<GlobalLeasingDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
