using Cassandra.Application.Contracts.BiayaBiroJasa;
using Cassandra.Application.DTOs.BiayaBiroJasa;

namespace Cassandra.Application.Queries.BiayaBiroJasa;

public record GetBiayaBiroJasasQuery;

public record GetBiayaBiroJasaByIdQuery(Guid Id);

public class GetBiayaBiroJasasQueryHandler(IBiayaBiroJasaQueryRepository repository)
{
    public Task<IReadOnlyList<BiayaBiroJasaDto>> HandleAsync(GetBiayaBiroJasasQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<BiayaBiroJasaDto?> HandleByIdAsync(GetBiayaBiroJasaByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
