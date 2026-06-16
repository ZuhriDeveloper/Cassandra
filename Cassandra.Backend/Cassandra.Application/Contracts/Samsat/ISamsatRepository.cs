using Cassandra.Domain.Samsat;

namespace Cassandra.Application.Contracts.Samsat;

public interface ISamsatRepository
{
    Task<Domain.Samsat.Samsat?> GetByIdAsync(SamsatId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Samsat.Samsat samsat, CancellationToken ct = default);
}
