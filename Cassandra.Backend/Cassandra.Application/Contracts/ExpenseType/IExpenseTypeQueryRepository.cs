using Cassandra.Application.DTOs.ExpenseType;

namespace Cassandra.Application.Contracts.ExpenseType;

public interface IExpenseTypeQueryRepository
{
    Task<ExpenseTypeDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ExpenseTypeDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
