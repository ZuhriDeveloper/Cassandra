using Cassandra.Domain.SoPenerimaan;

namespace Cassandra.Application.Contracts.SoPenerimaan;

public interface ISoPenerimaanRepository
{
    Task<Domain.SoPenerimaan.SoPenerimaan?> GetByIdAsync(SoPenerimaanId id, CancellationToken ct = default);
    Task SaveAsync(Domain.SoPenerimaan.SoPenerimaan soPenerimaan, CancellationToken ct = default);
}
