using Cassandra.Domain.Kelengkapan;

namespace Cassandra.Application.Contracts.Kelengkapan;

public interface IKelengkapanRepository
{
    Task<Domain.Kelengkapan.Kelengkapan?> GetByIdAsync(KelengkapanId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Kelengkapan.Kelengkapan kelengkapan, CancellationToken ct = default);
}
