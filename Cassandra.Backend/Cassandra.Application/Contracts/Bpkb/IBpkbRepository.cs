namespace Cassandra.Application.Contracts.Bpkb;

public interface IBpkbRepository
{
    Task<Domain.Bpkb.Bpkb?> GetByIdAsync(Domain.Bpkb.BpkbId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Bpkb.Bpkb bpkb, CancellationToken ct = default);
}
