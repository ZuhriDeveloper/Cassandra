using Cassandra.Application.Contracts.Warna;
using Cassandra.Application.DTOs.Warna;

namespace Cassandra.Application.Queries.Warna;

public record GetWarnasQuery;

public record GetWarnaByIdQuery(Guid Id);

public class GetWarnasQueryHandler(IWarnaQueryRepository repository)
{
    public Task<IReadOnlyList<WarnaDto>> HandleAsync(GetWarnasQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<WarnaDto?> HandleByIdAsync(GetWarnaByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
