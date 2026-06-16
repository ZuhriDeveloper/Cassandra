using Cassandra.Application.DTOs.PelanggaranWilayah;

namespace Cassandra.Application.Contracts.PelanggaranWilayah;

public interface IPelanggaranWilayahQueryRepository
{
    Task<PelanggaranWilayahDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<PelanggaranWilayahDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> AreaCodeExistsAsync(string areaCode, CancellationToken ct = default);
}
