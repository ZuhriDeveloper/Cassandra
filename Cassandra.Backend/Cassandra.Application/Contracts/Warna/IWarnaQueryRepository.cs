using Cassandra.Application.DTOs.Warna;

namespace Cassandra.Application.Contracts.Warna;

public interface IWarnaQueryRepository
{
    Task<WarnaDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<WarnaDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
}
