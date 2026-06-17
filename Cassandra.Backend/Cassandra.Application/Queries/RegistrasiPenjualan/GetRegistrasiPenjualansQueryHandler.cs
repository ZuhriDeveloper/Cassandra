using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.DTOs.RegistrasiPenjualan;

namespace Cassandra.Application.Queries.RegistrasiPenjualan;

public record GetRegistrasiPenjualansQuery;
public record GetRegistrasiPenjualanByIdQuery(Guid Id);

public class GetRegistrasiPenjualansQueryHandler(IRegistrasiPenjualanQueryRepository queryRepo)
{
    public Task<IReadOnlyList<RegistrasiPenjualanDto>> HandleAsync(
        GetRegistrasiPenjualansQuery query, CancellationToken ct = default)
        => queryRepo.GetAllAsync(ct);

    public Task<RegistrasiPenjualanDto?> HandleByIdAsync(
        GetRegistrasiPenjualanByIdQuery query, CancellationToken ct = default)
        => queryRepo.GetByIdAsync(query.Id, ct);
}
