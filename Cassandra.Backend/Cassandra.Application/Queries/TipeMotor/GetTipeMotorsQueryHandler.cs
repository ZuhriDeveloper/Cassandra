using Cassandra.Application.Contracts.TipeMotor;
using Cassandra.Application.DTOs.TipeMotor;

namespace Cassandra.Application.Queries.TipeMotor;

public record GetTipeMotorsQuery;

public record GetTipeMotorByIdQuery(Guid Id);

public class GetTipeMotorsQueryHandler(ITipeMotorQueryRepository repository)
{
    public Task<IReadOnlyList<TipeMotorDto>> HandleAsync(GetTipeMotorsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<TipeMotorDto?> HandleByIdAsync(GetTipeMotorByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
