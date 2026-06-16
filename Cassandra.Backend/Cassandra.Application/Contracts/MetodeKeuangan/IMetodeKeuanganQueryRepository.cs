using Cassandra.Application.DTOs.MetodeKeuangan;

namespace Cassandra.Application.Contracts.MetodeKeuangan;

public interface IMetodeKeuanganQueryRepository
{
    Task<MetodeKeuanganDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<MetodeKeuanganDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
