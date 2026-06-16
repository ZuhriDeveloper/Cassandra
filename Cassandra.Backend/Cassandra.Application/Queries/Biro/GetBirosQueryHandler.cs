using Cassandra.Application.Contracts.Biro;
using Cassandra.Application.DTOs.Biro;

namespace Cassandra.Application.Queries.Biro;

public record GetBirosQuery;

public record GetBiroByIdQuery(Guid Id);

public class GetBirosQueryHandler(IBiroQueryRepository repository)
{
    public Task<IReadOnlyList<BiroDto>> HandleAsync(GetBirosQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<BiroDto?> HandleByIdAsync(GetBiroByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
