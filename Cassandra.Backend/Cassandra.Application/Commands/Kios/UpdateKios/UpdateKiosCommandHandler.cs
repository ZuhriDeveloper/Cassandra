using Cassandra.Application.Contracts.Kios;
using Cassandra.Domain.Common;
using Cassandra.Domain.Kios;

namespace Cassandra.Application.Commands.Kios.UpdateKios;

public class UpdateKiosCommandHandler(
    IKiosRepository repository)
{
    public async Task HandleAsync(UpdateKiosCommand command, CancellationToken ct = default)
    {
        var kios = await repository.GetByIdAsync(KiosId.From(command.Id), ct)
            ?? throw new DomainException($"Kios dengan id '{command.Id}' tidak ditemukan.");

        kios.Update(command.Name, command.Phone, command.Fax, command.Address,
            command.PicKaryawanId, command.UpdatedBy);

        await repository.SaveAsync(kios, ct);
    }
}
