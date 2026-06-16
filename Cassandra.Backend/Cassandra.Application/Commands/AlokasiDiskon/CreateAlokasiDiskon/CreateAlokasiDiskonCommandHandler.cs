using Cassandra.Application.Contracts.AlokasiDiskon;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.AlokasiDiskon.CreateAlokasiDiskon;

public class CreateAlokasiDiskonCommandHandler(
    IAlokasiDiskonRepository repository,
    IAlokasiDiskonQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateAlokasiDiskonCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.KaryawanIdExistsAsync(command.KaryawanId, ct))
            throw new DomainException($"Alokasi diskon untuk karyawan ini sudah ada.");

        var ad = Domain.AlokasiDiskon.AlokasiDiskon.Create(command.KaryawanId, command.DiscountLevel, command.CreatedBy, dealerId);
        await repository.SaveAsync(ad, ct);
        return ad.Id.Value;
    }
}
