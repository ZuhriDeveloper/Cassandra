using Cassandra.Application.Contracts.Jabatan;
using Cassandra.Application.DTOs.Jabatan;

namespace Cassandra.Application.Queries.Jabatan;

public record GetJabatansQuery;

public record GetJabatanByIdQuery(Guid Id);

public class GetJabatansQueryHandler(IJabatanQueryRepository repository)
{
    public Task<IReadOnlyList<JabatanDto>> HandleAsync(GetJabatansQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<JabatanDto?> HandleByIdAsync(GetJabatanByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
