using Cassandra.Application.Contracts.Warna;
using Cassandra.Domain.Common;
using Cassandra.Domain.Warna;

namespace Cassandra.Application.Commands.Warna.SetWarnaStatus;

public class SetWarnaStatusCommandHandler(IWarnaRepository repository)
{
    public async Task HandleAsync(SetWarnaStatusCommand command, CancellationToken ct = default)
    {
        var warna = await repository.GetByIdAsync(WarnaId.From(command.Id), ct)
            ?? throw new DomainException($"Warna dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            warna.Activate(command.UpdatedBy);
        else
            warna.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(warna, ct);
    }
}
