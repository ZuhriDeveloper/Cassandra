using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Mediator;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Mediator.CreateMediator;

public class CreateMediatorCommandHandler(
    IMediatorRepository      repository,
    IMediatorQueryRepository queryRepository,
    ICurrentDealer           currentDealer)
{
    public async Task<Guid> HandleAsync(CreateMediatorCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Nama mediator '{command.Name}' sudah digunakan.");

        var mediator = Domain.Mediator.Mediator.Create(
            command.Name,
            command.KaryawanId,
            command.Address,
            command.Limit,
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(mediator, ct);
        return mediator.Id.Value;
    }
}
