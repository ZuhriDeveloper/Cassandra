using Cassandra.Application.DTOs.Df;

namespace Cassandra.Application.Contracts.Df;

public interface IDfQueryRepository
{
    Task<DfDto?> GetForDealerAsync(CancellationToken ct = default);
}
