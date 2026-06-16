using Cassandra.Application.Contracts.AlokasiDiskon;
using Cassandra.Application.DTOs.AlokasiDiskon;

namespace Cassandra.Application.Queries.AlokasiDiskon;

public record GetAlokasiDiskonsQuery;

public record GetAlokasiDiskonByIdQuery(Guid Id);

public class GetAlokasiDiskonsQueryHandler(IAlokasiDiskonQueryRepository repository)
{
    public Task<IReadOnlyList<AlokasiDiskonDto>> HandleAsync(GetAlokasiDiskonsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<AlokasiDiskonDto?> HandleByIdAsync(GetAlokasiDiskonByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
