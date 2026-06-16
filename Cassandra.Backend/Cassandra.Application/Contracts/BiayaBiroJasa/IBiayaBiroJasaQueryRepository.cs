using Cassandra.Application.DTOs.BiayaBiroJasa;

namespace Cassandra.Application.Contracts.BiayaBiroJasa;

public interface IBiayaBiroJasaQueryRepository
{
    Task<BiayaBiroJasaDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<BiayaBiroJasaDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> SamsatBiroExistsAsync(Guid samsatId, Guid biroId, CancellationToken ct = default);
}
