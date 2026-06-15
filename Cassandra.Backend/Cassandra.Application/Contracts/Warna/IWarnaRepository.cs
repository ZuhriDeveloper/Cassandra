using Cassandra.Domain.Warna;

namespace Cassandra.Application.Contracts.Warna;

public interface IWarnaRepository
{
    Task<Domain.Warna.Warna?> GetByIdAsync(WarnaId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Warna.Warna warna, CancellationToken ct = default);
}
