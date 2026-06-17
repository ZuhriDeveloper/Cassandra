using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.So;
using Cassandra.Domain.Common;
using Cassandra.Domain.So;

namespace Cassandra.Application.Commands.So.CreateSo;

public class CreateSoCommandHandler(
    ISoRepository repository,
    ISoQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateSoCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var soNumber = command.SoNumber.Trim().ToUpper();

        if (await queryRepository.SoNumberExistsAsync(soNumber, ct))
            throw new DomainException($"Nomor SO '{soNumber}' sudah ada.");

        var items = command.Items
            .Select(i => new SoItemValue(i.TipeMotorId, i.WarnaId, i.Qty, i.NettPrice))
            .ToList();

        var total = items.Sum(i => i.Qty * i.NettPrice);
        var qtyUnit = items.Sum(i => i.Qty);
        var subtotal = (total - command.Subsidi) * (command.CashDiscount / 100m);
        var ppn = (total - command.Subsidi - subtotal) * 0.1m;
        var totalAmount = total - command.Subsidi - subtotal + ppn;

        var so = Domain.So.So.Create(
            soNumber,
            command.SoDate,
            command.DueDate,
            command.PaymentType,
            command.MetodeKeuanganId,
            total,
            command.Subsidi,
            command.CashDiscount,
            ppn,
            totalAmount,
            command.Df,
            qtyUnit,
            items,
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(so, ct);
        return so.Id.Value;
    }
}
