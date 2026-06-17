using Cassandra.Application.DTOs.RegistrasiPenjualan;

namespace Cassandra.Application.Contracts.RegistrasiPenjualan;

public interface IRegistrasiPenjualanQueryRepository
{
    Task<IReadOnlyList<RegistrasiPenjualanDto>> GetAllAsync(CancellationToken ct = default);
    Task<RegistrasiPenjualanDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> NoPenjualanExistsAsync(string noPenjualan, CancellationToken ct = default);
}
