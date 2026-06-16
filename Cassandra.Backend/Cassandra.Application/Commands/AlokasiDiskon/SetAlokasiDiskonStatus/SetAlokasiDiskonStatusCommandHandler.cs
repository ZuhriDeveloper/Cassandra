using Cassandra.Application.Contracts.AlokasiDiskon;
using Cassandra.Domain.AlokasiDiskon;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.AlokasiDiskon.SetAlokasiDiskonStatus;

public class SetAlokasiDiskonStatusCommandHandler(IAlokasiDiskonRepository repository)
{
    public async Task HandleAsync(SetAlokasiDiskonStatusCommand command, CancellationToken ct = default)
    {
        var ad = await repository.GetByIdAsync(AlokasiDiskonId.From(command.Id), ct)
            ?? throw new DomainException($"Alokasi diskon dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            ad.Activate(command.ActionBy);
        else
            ad.Deactivate(command.ActionBy);

        await repository.SaveAsync(ad, ct);
    }
}
