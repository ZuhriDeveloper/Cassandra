using Cassandra.Application.Contracts.Stnk;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk;

namespace Cassandra.Application.Commands.Stnk.ReceiveStnk;

public class ReceiveStnkCommandHandler(IStnkRepository repository)
{
    public async Task HandleAsync(ReceiveStnkCommand command, CancellationToken ct = default)
    {
        var stnk = await repository.GetByIdAsync(StnkId.From(command.StnkId), ct);
        if (stnk is null)
            throw new DomainException($"STNK dengan ID '{command.StnkId}' tidak ditemukan.");

        stnk.Receive(
            command.ReceiveDate,
            command.PlateNumber,
            command.BiroId,
            command.StnkNumber,
            command.StnkCost,
            command.NoticeCost,
            command.ProgressiveCost,
            command.InvoiceNumber,
            command.Region,
            command.BbnCost,
            command.PnbpCost,
            command.AdminCost,
            command.OtherCost,
            command.ServiceCost,
            command.PphCost,
            command.IsInvoiceValid,
            command.UpdatedBy);

        await repository.SaveAsync(stnk, ct);
    }
}
