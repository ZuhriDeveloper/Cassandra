using Cassandra.Application.DTOs.TipeMotor;

namespace Cassandra.Application.Contracts.TipeMotor;

public interface ITipeMotorQueryRepository
{
    Task<TipeMotorDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TipeMotorDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
}
