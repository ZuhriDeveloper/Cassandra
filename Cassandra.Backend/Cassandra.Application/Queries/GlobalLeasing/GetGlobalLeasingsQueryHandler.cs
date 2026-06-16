using Cassandra.Application.Contracts.GlobalLeasing;
using Cassandra.Application.DTOs.GlobalLeasing;

namespace Cassandra.Application.Queries.GlobalLeasing;

public record GetGlobalLeasingsQuery;

public record GetGlobalLeasingByIdQuery(Guid Id);

public class GetGlobalLeasingsQueryHandler(IGlobalLeasingQueryRepository repository)
{
    public Task<IReadOnlyList<GlobalLeasingDto>> HandleAsync(GetGlobalLeasingsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<GlobalLeasingDto?> HandleByIdAsync(GetGlobalLeasingByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
