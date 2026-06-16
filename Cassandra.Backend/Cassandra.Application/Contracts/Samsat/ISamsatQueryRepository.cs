using Cassandra.Application.DTOs.Samsat;

namespace Cassandra.Application.Contracts.Samsat;

public interface ISamsatQueryRepository
{
    Task<SamsatDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SamsatDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
