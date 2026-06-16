namespace Cassandra.Application.DTOs.ExpenseType;

public record ExpenseTypeDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive);
