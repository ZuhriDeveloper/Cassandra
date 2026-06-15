using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.DTOs.Dealers;

namespace Cassandra.Application.Queries.Dealers;

public record GetDealersQuery;

public record GetDealerByIdQuery(Guid Id);

public class GetDealersQueryHandler(IDealerQueryRepository repository)
{
    public Task<IReadOnlyList<DealerDto>> HandleAsync(GetDealersQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<DealerDto?> HandleByIdAsync(GetDealerByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
