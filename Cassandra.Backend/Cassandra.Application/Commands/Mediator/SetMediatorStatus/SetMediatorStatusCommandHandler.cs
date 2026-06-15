using Cassandra.Application.Contracts.Mediator;
using Cassandra.Domain.Common;
using Cassandra.Domain.Mediator;

namespace Cassandra.Application.Commands.Mediator.SetMediatorStatus;

public class SetMediatorStatusCommandHandler(IMediatorRepository repository)
{
    public async Task HandleAsync(SetMediatorStatusCommand command, CancellationToken ct = default)
    {
        var mediator = await repository.GetByIdAsync(MediatorId.From(command.Id), ct)
            ?? throw new DomainException($"Mediator dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            mediator.Activate(command.ChangedBy);
        else
            mediator.Deactivate(command.ChangedBy);

        await repository.SaveAsync(mediator, ct);
    }
}
