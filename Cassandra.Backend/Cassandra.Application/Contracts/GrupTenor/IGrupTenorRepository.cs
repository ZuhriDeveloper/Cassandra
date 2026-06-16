using Cassandra.Domain.GrupTenor;

namespace Cassandra.Application.Contracts.GrupTenor;

public interface IGrupTenorRepository
{
    Task<Domain.GrupTenor.GrupTenor?> GetByIdAsync(GrupTenorId id, CancellationToken ct = default);
    Task SaveAsync(Domain.GrupTenor.GrupTenor grupTenor, CancellationToken ct = default);
}
