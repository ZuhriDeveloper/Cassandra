using Cassandra.Application.Contracts.Mediator;
using Cassandra.Domain.Common;
using Cassandra.Domain.Mediator;

namespace Cassandra.Application.Commands.Mediator.SetMediatorLimit;

public class SetMediatorLimitCommandHandler(IMediatorRepository repository)
{
    public async Task HandleAsync(SetMediatorLimitCommand command, CancellationToken ct = default)
    {
        var mediator = await repository.GetByIdAsync(MediatorId.From(command.Id), ct)
            ?? throw new DomainException($"Mediator dengan id '{command.Id}' tidak ditemukan.");

        mediator.SetLimit(command.Limit, command.SetBy);
        await repository.SaveAsync(mediator, ct);
    }
}
