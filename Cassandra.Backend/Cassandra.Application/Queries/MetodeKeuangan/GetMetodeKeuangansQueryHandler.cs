using Cassandra.Application.Contracts.MetodeKeuangan;
using Cassandra.Application.DTOs.MetodeKeuangan;

namespace Cassandra.Application.Queries.MetodeKeuangan;

public record GetMetodeKeuangansQuery;

public record GetMetodeKeuanganByIdQuery(Guid Id);

public class GetMetodeKeuangansQueryHandler(IMetodeKeuanganQueryRepository repository)
{
    public Task<IReadOnlyList<MetodeKeuanganDto>> HandleAsync(GetMetodeKeuangansQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<MetodeKeuanganDto?> HandleByIdAsync(GetMetodeKeuanganByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
