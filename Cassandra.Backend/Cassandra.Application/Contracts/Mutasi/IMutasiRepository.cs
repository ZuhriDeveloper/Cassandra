using Cassandra.Domain.Mutasi;

namespace Cassandra.Application.Contracts.Mutasi;

public interface IMutasiRepository
{
    Task<Domain.Mutasi.Mutasi?> GetByIdAsync(MutasiId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Mutasi.Mutasi mutasi, CancellationToken ct = default);
}
