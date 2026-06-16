using Cassandra.Domain.ExpenseType;

namespace Cassandra.Application.Contracts.ExpenseType;

public interface IExpenseTypeRepository
{
    Task<Domain.ExpenseType.ExpenseType?> GetByIdAsync(ExpenseTypeId id, CancellationToken ct = default);
    Task SaveAsync(Domain.ExpenseType.ExpenseType expenseType, CancellationToken ct = default);
}
