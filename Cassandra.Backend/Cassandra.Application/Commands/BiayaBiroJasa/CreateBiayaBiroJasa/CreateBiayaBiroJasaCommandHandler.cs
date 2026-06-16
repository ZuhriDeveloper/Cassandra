using Cassandra.Application.Contracts.BiayaBiroJasa;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.BiayaBiroJasa.CreateBiayaBiroJasa;

public class CreateBiayaBiroJasaCommandHandler(
    IBiayaBiroJasaRepository repository,
    IBiayaBiroJasaQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateBiayaBiroJasaCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.SamsatBiroExistsAsync(command.SamsatId, command.BiroId, ct))
            throw new DomainException("Kombinasi Samsat dan Biro sudah ada.");

        var bbj = Domain.BiayaBiroJasa.BiayaBiroJasa.Create(command.SamsatId, command.BiroId, command.CreatedBy, dealerId);
        await repository.SaveAsync(bbj, ct);
        return bbj.Id.Value;
    }
}
