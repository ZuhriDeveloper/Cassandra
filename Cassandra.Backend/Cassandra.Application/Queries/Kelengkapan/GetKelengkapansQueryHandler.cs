using Cassandra.Application.Contracts.Kelengkapan;
using Cassandra.Application.DTOs.Kelengkapan;

namespace Cassandra.Application.Queries.Kelengkapan;

public record GetKelengkapansQuery;

public record GetKelengkapanByIdQuery(Guid Id);

public class GetKelengkapansQueryHandler(IKelengkapanQueryRepository repository)
{
    public Task<IReadOnlyList<KelengkapanDto>> HandleAsync(GetKelengkapansQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<KelengkapanDto?> HandleByIdAsync(GetKelengkapanByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
