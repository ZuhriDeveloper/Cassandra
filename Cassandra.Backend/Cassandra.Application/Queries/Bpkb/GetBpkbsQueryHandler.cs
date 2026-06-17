using Cassandra.Application.Contracts.Bpkb;
using Cassandra.Application.DTOs.Bpkb;

namespace Cassandra.Application.Queries.Bpkb;

public record GetBpkbsQuery;
public record GetBpkbByIdQuery(Guid Id);

public class GetBpkbsQueryHandler(IBpkbQueryRepository queryRepo)
{
    public Task<IReadOnlyList<BpkbDto>> HandleAsync(
        GetBpkbsQuery query, CancellationToken ct = default)
        => queryRepo.GetAllAsync(ct);

    public Task<BpkbDto?> HandleByIdAsync(
        GetBpkbByIdQuery query, CancellationToken ct = default)
        => queryRepo.GetByIdAsync(query.Id, ct);
}
