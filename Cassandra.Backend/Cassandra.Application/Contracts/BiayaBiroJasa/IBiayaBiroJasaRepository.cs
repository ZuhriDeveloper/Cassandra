using Cassandra.Domain.BiayaBiroJasa;

namespace Cassandra.Application.Contracts.BiayaBiroJasa;

public interface IBiayaBiroJasaRepository
{
    Task<Domain.BiayaBiroJasa.BiayaBiroJasa?> GetByIdAsync(BiayaBiroJasaId id, CancellationToken ct = default);
    Task SaveAsync(Domain.BiayaBiroJasa.BiayaBiroJasa biayaBiroJasa, CancellationToken ct = default);
}
