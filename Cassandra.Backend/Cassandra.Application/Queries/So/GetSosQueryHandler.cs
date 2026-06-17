using Cassandra.Application.Contracts.So;
using Cassandra.Application.DTOs.So;

namespace Cassandra.Application.Queries.So;

public record GetSosQuery;
public record GetSoByIdQuery(Guid Id);

public class GetSosQueryHandler(ISoQueryRepository queryRepository)
{
    public Task<IReadOnlyList<SoDto>> HandleAsync(GetSosQuery query, CancellationToken ct = default)
        => queryRepository.GetAllAsync(ct);

    public Task<SoDto?> HandleByIdAsync(GetSoByIdQuery query, CancellationToken ct = default)
        => queryRepository.GetWithItemsAsync(query.Id, ct);
}
