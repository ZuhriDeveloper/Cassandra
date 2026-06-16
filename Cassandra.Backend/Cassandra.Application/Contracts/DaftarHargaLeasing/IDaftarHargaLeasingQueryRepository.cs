using Cassandra.Application.DTOs.DaftarHargaLeasing;

namespace Cassandra.Application.Contracts.DaftarHargaLeasing;

public interface IDaftarHargaLeasingQueryRepository
{
    Task<DaftarHargaLeasingDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<DaftarHargaLeasingDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(string name, Guid globalLeasingId, Guid grupTenorId, CancellationToken ct = default);
    Task<bool> ExistsExcludingAsync(string name, Guid globalLeasingId, Guid grupTenorId, Guid excludeId, CancellationToken ct = default);
}
