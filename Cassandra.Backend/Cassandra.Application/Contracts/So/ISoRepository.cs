using Cassandra.Domain.So;

namespace Cassandra.Application.Contracts.So;

public interface ISoRepository
{
    Task<Domain.So.So?> GetByIdAsync(SoId id, CancellationToken ct = default);
    Task SaveAsync(Domain.So.So so, CancellationToken ct = default);
}
