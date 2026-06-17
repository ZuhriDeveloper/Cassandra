namespace Cassandra.Application.Contracts.Stnk;

public interface IStnkRepository
{
    Task<Domain.Stnk.Stnk?> GetByIdAsync(Domain.Stnk.StnkId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Stnk.Stnk stnk, CancellationToken ct = default);
}
