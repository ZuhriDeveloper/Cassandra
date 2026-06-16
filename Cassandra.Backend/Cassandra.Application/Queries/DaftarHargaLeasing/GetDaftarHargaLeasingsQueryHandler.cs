using Cassandra.Application.Contracts.DaftarHargaLeasing;
using Cassandra.Application.DTOs.DaftarHargaLeasing;

namespace Cassandra.Application.Queries.DaftarHargaLeasing;

public record GetDaftarHargaLeasingsQuery;

public record GetDaftarHargaLeasingByIdQuery(Guid Id);

public class GetDaftarHargaLeasingsQueryHandler(IDaftarHargaLeasingQueryRepository repository)
{
    public Task<IReadOnlyList<DaftarHargaLeasingDto>> HandleAsync(GetDaftarHargaLeasingsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<DaftarHargaLeasingDto?> HandleByIdAsync(GetDaftarHargaLeasingByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
