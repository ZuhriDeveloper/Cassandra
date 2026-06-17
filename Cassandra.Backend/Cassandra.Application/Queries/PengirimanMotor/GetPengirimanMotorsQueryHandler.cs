using Cassandra.Application.Contracts.PengirimanMotor;
using Cassandra.Application.DTOs.PengirimanMotor;

namespace Cassandra.Application.Queries.PengirimanMotor;

public record GetPengirimanMotorsQuery;
public record GetPengirimanMotorByIdQuery(Guid Id);

public class GetPengirimanMotorsQueryHandler(IPengirimanMotorQueryRepository queryRepo)
{
    public Task<IReadOnlyList<PengirimanMotorDto>> HandleAsync(
        GetPengirimanMotorsQuery query, CancellationToken ct = default)
        => queryRepo.GetAllAsync(ct);

    public Task<PengirimanMotorDto?> HandleByIdAsync(
        GetPengirimanMotorByIdQuery query, CancellationToken ct = default)
        => queryRepo.GetByIdAsync(query.Id, ct);
}
