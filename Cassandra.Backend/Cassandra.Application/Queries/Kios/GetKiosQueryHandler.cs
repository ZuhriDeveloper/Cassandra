using Cassandra.Application.Contracts.Kios;
using Cassandra.Application.DTOs.Kios;

namespace Cassandra.Application.Queries.Kios;

public record GetKiosQuery;
public record GetKiosByIdQuery(Guid Id);

public class GetKiosQueryHandler(IKiosQueryRepository repository)
{
    public Task<IReadOnlyList<KiosDto>> HandleAsync(GetKiosQuery _, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<KiosDto?> HandleByIdAsync(GetKiosByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
