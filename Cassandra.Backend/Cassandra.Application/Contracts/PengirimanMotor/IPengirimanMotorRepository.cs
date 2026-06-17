using Cassandra.Domain.PengirimanMotor;

namespace Cassandra.Application.Contracts.PengirimanMotor;

public interface IPengirimanMotorRepository
{
    Task<Domain.PengirimanMotor.PengirimanMotor?> GetByIdAsync(PengirimanMotorId id, CancellationToken ct = default);
    Task SaveAsync(Domain.PengirimanMotor.PengirimanMotor pengiriman, CancellationToken ct = default);
}
