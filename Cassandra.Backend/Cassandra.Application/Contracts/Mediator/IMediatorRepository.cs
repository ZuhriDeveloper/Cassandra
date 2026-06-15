using Cassandra.Domain.Mediator;

namespace Cassandra.Application.Contracts.Mediator;

public interface IMediatorRepository
{
    Task<Domain.Mediator.Mediator?> GetByIdAsync(MediatorId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Mediator.Mediator mediator, CancellationToken ct = default);
}
