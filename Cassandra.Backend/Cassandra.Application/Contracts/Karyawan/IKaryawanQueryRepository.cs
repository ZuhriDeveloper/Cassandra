using Cassandra.Application.DTOs.Karyawan;

namespace Cassandra.Application.Contracts.Karyawan;

public interface IKaryawanQueryRepository
{
    Task<KaryawanDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<KaryawanDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> EmailExistsExcludingAsync(string email, Guid excludeId, CancellationToken ct = default);
}
