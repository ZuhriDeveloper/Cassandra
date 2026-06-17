using Cassandra.Application.DTOs.Bpkb;

namespace Cassandra.Application.Contracts.Bpkb;

public interface IBpkbQueryRepository
{
    Task<IReadOnlyList<BpkbDto>> GetAllAsync(CancellationToken ct = default);
    Task<BpkbDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
