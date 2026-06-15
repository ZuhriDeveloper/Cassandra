using Cassandra.Application.Contracts.GrupTipeMotor;
using Cassandra.Application.DTOs.GrupTipeMotor;

namespace Cassandra.Application.Queries.GrupTipeMotor;

public record GetGrupTipeMotorsQuery;

public record GetGrupTipeMotorByIdQuery(Guid Id);

public class GetGrupTipeMotorsQueryHandler(IGrupTipeMotorQueryRepository repository)
{
    public Task<IReadOnlyList<GrupTipeMotorDto>> HandleAsync(GetGrupTipeMotorsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<GrupTipeMotorDto?> HandleByIdAsync(GetGrupTipeMotorByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
