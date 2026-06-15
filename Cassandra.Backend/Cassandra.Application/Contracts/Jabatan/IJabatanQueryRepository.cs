using Cassandra.Application.DTOs.Jabatan;

namespace Cassandra.Application.Contracts.Jabatan;

public interface IJabatanQueryRepository
{
    Task<JabatanDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<JabatanDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
}
