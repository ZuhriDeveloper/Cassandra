using Cassandra.Domain.GrupTipeMotor;

namespace Cassandra.Application.Contracts.GrupTipeMotor;

public interface IGrupTipeMotorRepository
{
    Task<Domain.GrupTipeMotor.GrupTipeMotor?> GetByIdAsync(GrupTipeMotorId id, CancellationToken ct = default);
    Task SaveAsync(Domain.GrupTipeMotor.GrupTipeMotor grup, CancellationToken ct = default);
}
