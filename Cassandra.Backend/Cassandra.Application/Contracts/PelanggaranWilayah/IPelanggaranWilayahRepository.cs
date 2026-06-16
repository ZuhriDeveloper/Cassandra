using Cassandra.Domain.PelanggaranWilayah;

namespace Cassandra.Application.Contracts.PelanggaranWilayah;

public interface IPelanggaranWilayahRepository
{
    Task<Domain.PelanggaranWilayah.PelanggaranWilayah?> GetByIdAsync(PelanggaranWilayahId id, CancellationToken ct = default);
    Task SaveAsync(Domain.PelanggaranWilayah.PelanggaranWilayah pelanggaranWilayah, CancellationToken ct = default);
}
