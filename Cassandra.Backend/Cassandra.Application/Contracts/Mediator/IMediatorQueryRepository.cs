using Cassandra.Application.DTOs.Mediator;

namespace Cassandra.Application.Contracts.Mediator;

public interface IMediatorQueryRepository
{
    Task<MediatorDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<MediatorDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
