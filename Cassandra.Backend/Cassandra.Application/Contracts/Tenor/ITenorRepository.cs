using Cassandra.Domain.Tenor;

namespace Cassandra.Application.Contracts.Tenor;

public interface ITenorRepository
{
    Task<Domain.Tenor.Tenor?> GetByIdAsync(TenorId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Tenor.Tenor tenor, CancellationToken ct = default);
}
