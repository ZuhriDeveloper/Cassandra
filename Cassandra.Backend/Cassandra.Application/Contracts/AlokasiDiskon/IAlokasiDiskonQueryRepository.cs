using Cassandra.Application.DTOs.AlokasiDiskon;

namespace Cassandra.Application.Contracts.AlokasiDiskon;

public interface IAlokasiDiskonQueryRepository
{
    Task<AlokasiDiskonDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AlokasiDiskonDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> KaryawanIdExistsAsync(Guid karyawanId, CancellationToken ct = default);
}
