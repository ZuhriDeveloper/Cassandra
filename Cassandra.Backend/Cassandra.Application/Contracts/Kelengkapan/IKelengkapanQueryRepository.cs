using Cassandra.Application.DTOs.Kelengkapan;

namespace Cassandra.Application.Contracts.Kelengkapan;

public interface IKelengkapanQueryRepository
{
    Task<KelengkapanDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<KelengkapanDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
