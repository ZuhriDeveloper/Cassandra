using Cassandra.Domain.SoRetur;

namespace Cassandra.Application.Contracts.SoRetur;

public interface ISoReturRepository
{
    Task<Domain.SoRetur.SoRetur?> GetByIdAsync(SoReturId id, CancellationToken ct = default);
    Task SaveAsync(Domain.SoRetur.SoRetur soRetur, CancellationToken ct = default);
}
