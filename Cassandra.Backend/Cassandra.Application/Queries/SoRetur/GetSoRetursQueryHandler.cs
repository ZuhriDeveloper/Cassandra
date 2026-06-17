using Cassandra.Application.Contracts.SoRetur;
using Cassandra.Application.DTOs.SoRetur;

namespace Cassandra.Application.Queries.SoRetur;

public record GetSoRetursQuery;
public record GetSoReturByIdQuery(Guid Id);

public class GetSoRetursQueryHandler(ISoReturQueryRepository queryRepository)
{
    public Task<IReadOnlyList<SoReturDto>> HandleAsync(GetSoRetursQuery query, CancellationToken ct = default)
        => queryRepository.GetAllAsync(ct);

    public Task<SoReturDto?> HandleByIdAsync(GetSoReturByIdQuery query, CancellationToken ct = default)
        => queryRepository.GetWithItemsAsync(query.Id, ct);
}
