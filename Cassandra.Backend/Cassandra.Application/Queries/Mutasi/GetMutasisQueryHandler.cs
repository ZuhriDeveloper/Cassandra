using Cassandra.Application.Contracts.Mutasi;
using Cassandra.Application.DTOs.Mutasi;

namespace Cassandra.Application.Queries.Mutasi;

public record GetMutasisQuery;
public record GetMutasiByIdQuery(Guid Id);

public class GetMutasisQueryHandler(IMutasiQueryRepository queryRepository)
{
    public Task<IReadOnlyList<MutasiDto>> HandleAsync(GetMutasisQuery query, CancellationToken ct = default)
        => queryRepository.GetAllAsync(ct);

    public Task<MutasiDto?> HandleByIdAsync(GetMutasiByIdQuery query, CancellationToken ct = default)
        => queryRepository.GetWithItemsAsync(query.Id, ct);
}
