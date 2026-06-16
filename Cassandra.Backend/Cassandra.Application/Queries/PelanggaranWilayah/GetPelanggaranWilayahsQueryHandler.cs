using Cassandra.Application.Contracts.PelanggaranWilayah;
using Cassandra.Application.DTOs.PelanggaranWilayah;

namespace Cassandra.Application.Queries.PelanggaranWilayah;

public record GetPelanggaranWilayahsQuery;

public record GetPelanggaranWilayahByIdQuery(Guid Id);

public class GetPelanggaranWilayahsQueryHandler(IPelanggaranWilayahQueryRepository repository)
{
    public Task<IReadOnlyList<PelanggaranWilayahDto>> HandleAsync(GetPelanggaranWilayahsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<PelanggaranWilayahDto?> HandleByIdAsync(GetPelanggaranWilayahByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
