using Cassandra.Application.Contracts.Samsat;
using Cassandra.Application.DTOs.Samsat;

namespace Cassandra.Application.Queries.Samsat;

public record GetSamsatsQuery;

public record GetSamsatByIdQuery(Guid Id);

public class GetSamsatsQueryHandler(ISamsatQueryRepository repository)
{
    public Task<IReadOnlyList<SamsatDto>> HandleAsync(GetSamsatsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<SamsatDto?> HandleByIdAsync(GetSamsatByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
