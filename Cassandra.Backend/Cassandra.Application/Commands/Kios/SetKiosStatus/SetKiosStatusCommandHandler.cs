using Cassandra.Application.Contracts.Kios;
using Cassandra.Domain.Common;
using Cassandra.Domain.Kios;

namespace Cassandra.Application.Commands.Kios.SetKiosStatus;

public class SetKiosStatusCommandHandler(IKiosRepository repository)
{
    public async Task HandleAsync(SetKiosStatusCommand command, CancellationToken ct = default)
    {
        var kios = await repository.GetByIdAsync(KiosId.From(command.Id), ct)
            ?? throw new DomainException($"Kios dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            kios.Activate(command.ChangedBy);
        else
            kios.Deactivate(command.ChangedBy);

        await repository.SaveAsync(kios, ct);
    }
}
