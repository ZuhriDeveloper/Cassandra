using Cassandra.Application.DTOs.PengirimanMotor;

namespace Cassandra.Application.Contracts.PengirimanMotor;

public interface IPengirimanMotorQueryRepository
{
    Task<IReadOnlyList<PengirimanMotorDto>> GetAllAsync(CancellationToken ct = default);
    Task<PengirimanMotorDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
