using Cassandra.Domain.Kios;

namespace Cassandra.Application.Contracts.Kios;

public interface IKiosRepository
{
    Task<Domain.Kios.Kios?> GetByIdAsync(KiosId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Kios.Kios kios, CancellationToken ct = default);
}
