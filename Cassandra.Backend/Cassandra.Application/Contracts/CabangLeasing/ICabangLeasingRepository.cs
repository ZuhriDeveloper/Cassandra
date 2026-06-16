using Cassandra.Domain.CabangLeasing;

namespace Cassandra.Application.Contracts.CabangLeasing;

public interface ICabangLeasingRepository
{
    Task<Domain.CabangLeasing.CabangLeasing?> GetByIdAsync(CabangLeasingId id, CancellationToken ct = default);
    Task SaveAsync(Domain.CabangLeasing.CabangLeasing cabangLeasing, CancellationToken ct = default);
}
