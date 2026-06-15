using Cassandra.Domain.TipeMotor;

namespace Cassandra.Application.Contracts.TipeMotor;

public interface ITipeMotorRepository
{
    Task<Domain.TipeMotor.TipeMotor?> GetByIdAsync(TipeMotorId id, CancellationToken ct = default);
    Task SaveAsync(Domain.TipeMotor.TipeMotor tipe, CancellationToken ct = default);
}
