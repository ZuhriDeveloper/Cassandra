using Cassandra.Domain.Biro;

namespace Cassandra.Application.Contracts.Biro;

public interface IBiroRepository
{
    Task<Domain.Biro.Biro?> GetByIdAsync(BiroId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Biro.Biro biro, CancellationToken ct = default);
}
