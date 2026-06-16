using Cassandra.Domain.MetodeKeuangan;

namespace Cassandra.Application.Contracts.MetodeKeuangan;

public interface IMetodeKeuanganRepository
{
    Task<Domain.MetodeKeuangan.MetodeKeuangan?> GetByIdAsync(MetodeKeuanganId id, CancellationToken ct = default);
    Task SaveAsync(Domain.MetodeKeuangan.MetodeKeuangan metodeKeuangan, CancellationToken ct = default);
}
