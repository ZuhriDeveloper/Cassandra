using Cassandra.Application.Contracts.AlokasiDiskon;
using Cassandra.Domain.AlokasiDiskon;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.AlokasiDiskon.UpdateAlokasiDiskon;

public class UpdateAlokasiDiskonCommandHandler(IAlokasiDiskonRepository repository)
{
    public async Task HandleAsync(UpdateAlokasiDiskonCommand command, CancellationToken ct = default)
    {
        var ad = await repository.GetByIdAsync(AlokasiDiskonId.From(command.Id), ct)
            ?? throw new DomainException($"Alokasi diskon dengan id '{command.Id}' tidak ditemukan.");

        ad.Update(command.DiscountLevel, command.UpdatedBy);
        await repository.SaveAsync(ad, ct);
    }
}
