using Cassandra.Application.Contracts.Mediator;
using Cassandra.Application.DTOs.Mediator;

namespace Cassandra.Application.Queries.Mediator;

public record GetMediatorsQuery;
public record GetMediatorByIdQuery(Guid Id);

public class GetMediatorsQueryHandler(IMediatorQueryRepository repository)
{
    public Task<IReadOnlyList<MediatorDto>> HandleAsync(GetMediatorsQuery _, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<MediatorDto?> HandleByIdAsync(GetMediatorByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
