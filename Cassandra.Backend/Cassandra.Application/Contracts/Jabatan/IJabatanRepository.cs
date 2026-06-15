using Cassandra.Domain.Jabatan;

namespace Cassandra.Application.Contracts.Jabatan;

public interface IJabatanRepository
{
    Task<Domain.Jabatan.Jabatan?> GetByIdAsync(JabatanId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Jabatan.Jabatan jabatan, CancellationToken ct = default);
}
