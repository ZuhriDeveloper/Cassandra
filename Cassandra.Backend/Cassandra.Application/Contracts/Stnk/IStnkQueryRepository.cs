using Cassandra.Application.DTOs.Stnk;

namespace Cassandra.Application.Contracts.Stnk;

public interface IStnkQueryRepository
{
    Task<IReadOnlyList<StnkDto>> GetAllAsync(CancellationToken ct = default);
    Task<StnkDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByRegistrasiPenjualanIdAsync(Guid registrasiPenjualanId, CancellationToken ct = default);
}
