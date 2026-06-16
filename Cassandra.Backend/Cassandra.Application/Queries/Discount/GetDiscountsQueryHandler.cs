using Cassandra.Application.Contracts.Discount;
using Cassandra.Application.DTOs.Discount;

namespace Cassandra.Application.Queries.Discount;

public record GetDiscountsQuery;

public record GetDiscountByIdQuery(Guid Id);

public class GetDiscountsQueryHandler(IDiscountQueryRepository repository)
{
    public Task<IReadOnlyList<DiscountDto>> HandleAsync(GetDiscountsQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<DiscountDto?> HandleByIdAsync(GetDiscountByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
