using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.PelanggaranWilayah;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.PelanggaranWilayah.CreatePelanggaranWilayah;

public class CreatePelanggaranWilayahCommandHandler(
    IPelanggaranWilayahRepository repository,
    IPelanggaranWilayahQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreatePelanggaranWilayahCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.AreaCodeExistsAsync(command.AreaCode.Trim(), ct))
            throw new DomainException($"Kode area '{command.AreaCode.Trim()}' sudah ada.");

        var pw = Domain.PelanggaranWilayah.PelanggaranWilayah.Create(command.AreaCode, command.ExtraFee, command.CreatedBy, dealerId);
        await repository.SaveAsync(pw, ct);
        return pw.Id.Value;
    }
}
