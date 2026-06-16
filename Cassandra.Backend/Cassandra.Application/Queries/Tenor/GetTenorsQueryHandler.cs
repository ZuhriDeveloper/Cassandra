using Cassandra.Application.Contracts.Tenor;
using Cassandra.Application.DTOs.Tenor;

namespace Cassandra.Application.Queries.Tenor;

public record GetTenorsQuery;

public record GetTenorByIdQuery(Guid Id);

public class GetTenorsQueryHandler(ITenorQueryRepository repository)
{
    public Task<IReadOnlyList<TenorDto>> HandleAsync(GetTenorsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<TenorDto?> HandleByIdAsync(GetTenorByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
