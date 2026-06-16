using Cassandra.Application.Contracts.CabangLeasing;
using Cassandra.Application.DTOs.CabangLeasing;

namespace Cassandra.Application.Queries.CabangLeasing;

public record GetCabangLeasingsQuery;

public record GetCabangLeasingByIdQuery(Guid Id);

public class GetCabangLeasingsQueryHandler(ICabangLeasingQueryRepository repository)
{
    public Task<IReadOnlyList<CabangLeasingDto>> HandleAsync(GetCabangLeasingsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<CabangLeasingDto?> HandleByIdAsync(GetCabangLeasingByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
