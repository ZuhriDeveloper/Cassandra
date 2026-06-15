using Cassandra.Domain.Karyawan;

namespace Cassandra.Application.Contracts.Karyawan;

public interface IKaryawanRepository
{
    Task<Domain.Karyawan.Karyawan?> GetByIdAsync(KaryawanId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Karyawan.Karyawan karyawan, CancellationToken ct = default);
}
