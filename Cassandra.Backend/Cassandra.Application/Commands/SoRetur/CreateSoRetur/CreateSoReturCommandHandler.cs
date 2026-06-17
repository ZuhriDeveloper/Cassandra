using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.SoRetur;
using Cassandra.Domain.Common;
using Cassandra.Domain.SoRetur;

namespace Cassandra.Application.Commands.SoRetur.CreateSoRetur;

public class CreateSoReturCommandHandler(
    ISoReturRepository repository,
    ISoReturQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateSoReturCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var returNumber = command.ReturNumber.Trim().ToUpper();

        if (await queryRepository.ReturNumberExistsAsync(returNumber, ct))
            throw new DomainException($"Nomor retur '{returNumber}' sudah ada.");

        var items = command.Items
            .Select(i => new SoReturItemValue(i.TipeMotorId, i.WarnaId, i.Qty, i.NettPrice))
            .ToList();

        var retur = Domain.SoRetur.SoRetur.Create(
            returNumber,
            command.SoId,
            command.ReturDate,
            command.Reason,
            items,
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(retur, ct);
        return retur.Id.Value;
    }
}
