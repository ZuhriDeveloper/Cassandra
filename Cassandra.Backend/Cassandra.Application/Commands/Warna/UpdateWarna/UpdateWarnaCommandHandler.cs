using Cassandra.Application.Contracts.Warna;
using Cassandra.Domain.Common;
using Cassandra.Domain.Warna;

namespace Cassandra.Application.Commands.Warna.UpdateWarna;

public class UpdateWarnaCommandHandler(IWarnaRepository repository)
{
    public async Task HandleAsync(UpdateWarnaCommand command, CancellationToken ct = default)
    {
        var warna = await repository.GetByIdAsync(WarnaId.From(command.Id), ct)
            ?? throw new DomainException($"Warna dengan id '{command.Id}' tidak ditemukan.");

        warna.Update(command.Name, command.UpdatedBy);
        await repository.SaveAsync(warna, ct);
    }
}
