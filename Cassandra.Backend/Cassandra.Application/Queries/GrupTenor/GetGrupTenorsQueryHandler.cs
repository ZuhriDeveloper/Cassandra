using Cassandra.Application.Contracts.GrupTenor;
using Cassandra.Application.DTOs.GrupTenor;

namespace Cassandra.Application.Queries.GrupTenor;

public record GetGrupTenorsQuery;

public record GetGrupTenorByIdQuery(Guid Id);

public class GetGrupTenorsQueryHandler(IGrupTenorQueryRepository repository)
{
    public Task<IReadOnlyList<GrupTenorDto>> HandleAsync(GetGrupTenorsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<GrupTenorDto?> HandleByIdAsync(GetGrupTenorByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
