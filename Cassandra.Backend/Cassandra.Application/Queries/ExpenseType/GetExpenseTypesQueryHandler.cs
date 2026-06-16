using Cassandra.Application.Contracts.ExpenseType;
using Cassandra.Application.DTOs.ExpenseType;

namespace Cassandra.Application.Queries.ExpenseType;

public record GetExpenseTypesQuery;

public record GetExpenseTypeByIdQuery(Guid Id);

public class GetExpenseTypesQueryHandler(IExpenseTypeQueryRepository repository)
{
    public Task<IReadOnlyList<ExpenseTypeDto>> HandleAsync(GetExpenseTypesQuery query, CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public Task<ExpenseTypeDto?> HandleByIdAsync(GetExpenseTypeByIdQuery query, CancellationToken ct = default)
        => repository.GetByIdAsync(query.Id, ct);
}
