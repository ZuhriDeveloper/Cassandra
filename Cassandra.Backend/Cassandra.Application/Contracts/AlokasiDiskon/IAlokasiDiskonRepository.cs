using Cassandra.Domain.AlokasiDiskon;

namespace Cassandra.Application.Contracts.AlokasiDiskon;

public interface IAlokasiDiskonRepository
{
    Task<Domain.AlokasiDiskon.AlokasiDiskon?> GetByIdAsync(AlokasiDiskonId id, CancellationToken ct = default);
    Task SaveAsync(Domain.AlokasiDiskon.AlokasiDiskon alokasiDiskon, CancellationToken ct = default);
}
