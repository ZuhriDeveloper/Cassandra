using Cassandra.Domain.DaftarHargaLeasing;

namespace Cassandra.Application.Contracts.DaftarHargaLeasing;

public interface IDaftarHargaLeasingRepository
{
    Task<Domain.DaftarHargaLeasing.DaftarHargaLeasing?> GetByIdAsync(DaftarHargaLeasingId id, CancellationToken ct = default);
    Task SaveAsync(Domain.DaftarHargaLeasing.DaftarHargaLeasing daftarHargaLeasing, CancellationToken ct = default);
}
