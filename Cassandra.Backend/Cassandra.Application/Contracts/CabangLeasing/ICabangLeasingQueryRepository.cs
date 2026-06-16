using Cassandra.Application.DTOs.CabangLeasing;

namespace Cassandra.Application.Contracts.CabangLeasing;

public interface ICabangLeasingQueryRepository
{
    Task<CabangLeasingDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CabangLeasingDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
