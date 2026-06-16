using Cassandra.Application.Contracts.DiscountCash;
using Cassandra.Application.DTOs.DiscountCash;

namespace Cassandra.Application.Queries.DiscountCash;

public record GetDiscountCashsQuery;

public record GetDiscountCashByIdQuery(Guid Id);

public class GetDiscountCashsQueryHandler(IDiscountCashQueryRepository repository)
{
    public Task<IReadOnlyList<DiscountCashDto>> HandleAsync(GetDiscountCashsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<DiscountCashDto?> HandleByIdAsync(GetDiscountCashByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
