using Cassandra.Application.Contracts.ExpenseType;
using Cassandra.Application.DTOs.ExpenseType;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.ExpenseType;

public class ExpenseTypeQueryRepository(AppDbContext context) : IExpenseTypeQueryRepository
{
    public async Task<ExpenseTypeDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.ExpenseTypeReadModels.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new ExpenseTypeDto(x.Id, x.Code, x.Name, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ExpenseTypeDto>> GetAllAsync(CancellationToken ct = default)
        => await context.ExpenseTypeReadModels.AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new ExpenseTypeDto(x.Id, x.Code, x.Name, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.ExpenseTypeReadModels.IgnoreQueryFilters().AnyAsync(x => x.Code == code, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.ExpenseTypeReadModels.AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.ExpenseTypeReadModels.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);
}
