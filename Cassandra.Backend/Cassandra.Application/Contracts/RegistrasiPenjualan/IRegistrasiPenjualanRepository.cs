using Cassandra.Domain.RegistrasiPenjualan;

namespace Cassandra.Application.Contracts.RegistrasiPenjualan;

public interface IRegistrasiPenjualanRepository
{
    Task<Domain.RegistrasiPenjualan.RegistrasiPenjualan?> GetByIdAsync(RegistrasiPenjualanId id, CancellationToken ct = default);
    Task SaveAsync(Domain.RegistrasiPenjualan.RegistrasiPenjualan registrasi, CancellationToken ct = default);
}
