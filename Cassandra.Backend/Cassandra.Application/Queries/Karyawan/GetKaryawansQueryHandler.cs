using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Application.DTOs.Karyawan;

namespace Cassandra.Application.Queries.Karyawan;

public record GetKaryawansQuery;
public record GetKaryawanByIdQuery(Guid Id);

public class GetKaryawansQueryHandler(IKaryawanQueryRepository repository)
{
    public Task<IReadOnlyList<KaryawanDto>> HandleAsync(GetKaryawansQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<KaryawanDto?> HandleByIdAsync(GetKaryawanByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
