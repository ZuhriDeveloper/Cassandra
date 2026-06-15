using Cassandra.Application.DTOs.GrupTipeMotor;

namespace Cassandra.Application.Contracts.GrupTipeMotor;

public interface IGrupTipeMotorQueryRepository
{
    Task<GrupTipeMotorDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<GrupTipeMotorDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
}
