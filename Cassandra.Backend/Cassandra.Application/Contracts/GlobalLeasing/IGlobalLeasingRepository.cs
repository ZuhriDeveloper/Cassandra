using Cassandra.Domain.GlobalLeasing;

namespace Cassandra.Application.Contracts.GlobalLeasing;

public interface IGlobalLeasingRepository
{
    Task<Domain.GlobalLeasing.GlobalLeasing?> GetByIdAsync(GlobalLeasingId id, CancellationToken ct = default);
    Task SaveAsync(Domain.GlobalLeasing.GlobalLeasing globalLeasing, CancellationToken ct = default);
}
