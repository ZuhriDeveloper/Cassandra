using Cassandra.Application.Contracts.Mediator;
using Cassandra.Domain.Common;
using Cassandra.Domain.Mediator;

namespace Cassandra.Application.Commands.Mediator.UpdateMediator;

public class UpdateMediatorCommandHandler(
    IMediatorRepository      repository,
    IMediatorQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateMediatorCommand command, CancellationToken ct = default)
    {
        var mediator = await repository.GetByIdAsync(MediatorId.From(command.Id), ct)
            ?? throw new DomainException($"Mediator dengan id '{command.Id}' tidak ditemukan.");

        if (!string.Equals(mediator.Name, command.Name.Trim(), StringComparison.OrdinalIgnoreCase)
            && await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Nama mediator '{command.Name}' sudah digunakan.");

        mediator.Update(command.Name, command.KaryawanId, command.Address, command.UpdatedBy);
        await repository.SaveAsync(mediator, ct);
    }
}
