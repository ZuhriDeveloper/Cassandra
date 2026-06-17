using Cassandra.Application.Contracts.ApTransaction;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.Contracts.Stnk;
using Cassandra.Domain.ApTransaction;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk;
using Domain = Cassandra.Domain;

namespace Cassandra.Application.Commands.Stnk.ReceiveStnk;

public class ReceiveStnkCommandHandler(
    IStnkRepository repository,
    IApTransactionRepository apTransactionRepo,
    IRegistrasiPenjualanQueryRepository registrasiQueryRepo,
    ICurrentDealer currentDealer)
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

        // ── Create AP transactions for STNK fees ──────────────────────────────
        var dealerId = currentDealer.DealerId;

        // Get NoRangka from related RegistrasiPenjualan
        var registrasiDto = await registrasiQueryRepo.GetByIdAsync(stnk.RegistrasiPenjualanId, ct);
        var noRangka = registrasiDto?.NoRangka ?? string.Empty;

        var stnkTotal = command.BbnCost + command.NoticeCost;
        if (stnkTotal > 0)
        {
            var ap = Domain.ApTransaction.ApTransaction.Create(
                ApTransactionId.New(),
                Domain.ApTransaction.ApTransaction.STNK,
                command.StnkId,
                noRangka,
                stnkTotal,
                command.UpdatedBy,
                dealerId);
            await apTransactionRepo.SaveAsync(ap, ct);
        }

        if (command.PphCost > 0)
        {
            var apPph = Domain.ApTransaction.ApTransaction.Create(
                ApTransactionId.New(),
                Domain.ApTransaction.ApTransaction.PPH_STNK,
                command.StnkId,
                noRangka,
                command.PphCost,
                command.UpdatedBy,
                dealerId);
            await apTransactionRepo.SaveAsync(apPph, ct);
        }

        if (command.ProgressiveCost > 0)
        {
            var apProg = Domain.ApTransaction.ApTransaction.Create(
                ApTransactionId.New(),
                Domain.ApTransaction.ApTransaction.PROGRESSIVE_STNK,
                command.StnkId,
                noRangka,
                command.ProgressiveCost,
                command.UpdatedBy,
                dealerId);
            await apTransactionRepo.SaveAsync(apProg, ct);
        }
    }
}
