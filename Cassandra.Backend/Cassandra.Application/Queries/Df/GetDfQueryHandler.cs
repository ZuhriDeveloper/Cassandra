using Cassandra.Application.Contracts.Df;
using Cassandra.Application.DTOs.Df;

namespace Cassandra.Application.Queries.Df;

public record GetDfQuery;

public class GetDfQueryHandler(IDfQueryRepository repository)
{
    public Task<DfDto?> HandleAsync(GetDfQuery query, CancellationToken ct = default)
        => repository.GetForDealerAsync(ct);
}
