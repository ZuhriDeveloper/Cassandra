using Cassandra.Application.Contracts.Stnk;
using Cassandra.Application.DTOs.Stnk;

namespace Cassandra.Application.Queries.Stnk;

public record GetStnksQuery;
public record GetStnkByIdQuery(Guid Id);

public class GetStnksQueryHandler(IStnkQueryRepository queryRepo)
{
    public Task<IReadOnlyList<StnkDto>> HandleAsync(
        GetStnksQuery query, CancellationToken ct = default)
        => queryRepo.GetAllAsync(ct);

    public Task<StnkDto?> HandleByIdAsync(
        GetStnkByIdQuery query, CancellationToken ct = default)
        => queryRepo.GetByIdAsync(query.Id, ct);
}
