using Cassandra.Application.Contracts.ArTransaction;
using Cassandra.Application.Contracts.CashOutTransaction;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Finance;
using Cassandra.Application.Contracts.So;
using Cassandra.Application.Contracts.SoRetur;
using Cassandra.Domain.ArTransaction;
using Cassandra.Domain.CashOutTransaction;
using Cassandra.Domain.Common;
using Cassandra.Domain.So;
using Cassandra.Domain.SoRetur;
using Domain = Cassandra.Domain;

namespace Cassandra.Application.Commands.SoRetur.CreateSoRetur;

public class CreateSoReturCommandHandler(
    ISoReturRepository repository,
    ISoReturQueryRepository queryRepository,
    ICashOutTransactionRepository cashOutRepo,
    IArTransactionRepository arTransactionRepo,
    IFinanceCounter financeCounter,
    ISoQueryRepository soQueryRepository,
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

        // ── CashOut for retur payment ─────────────────────────────────────────
        var soDto = await soQueryRepository.GetByIdAsync(retur.SoId, ct);
        if (soDto is not null && retur.TotalAmount > 0)
        {
            var transType = soDto.PaymentType == SoPaymentType.CASH
                ? Domain.CashOutTransaction.CashOutTransaction.FSO_RETUR_CASH
                : Domain.CashOutTransaction.CashOutTransaction.FSO_RETUR_DF;
            var fInvoiceId = await financeCounter.GetNextFInvoiceIdAsync(ct);
            var cashOut = Domain.CashOutTransaction.CashOutTransaction.Create(
                CashOutTransactionId.New(),
                transType,
                retur.SoId,
                retur.Id.Value,
                retur.TotalAmount,
                0m,
                0,
                DateTime.UtcNow,
                "TRANSFER",
                fInvoiceId,
                command.CreatedBy,
                dealerId);
            await cashOutRepo.SaveAsync(cashOut, ct);
        }

        // ── AR for SO_RETUR (dealer receivable from HO) ───────────────────────
        if (retur.TotalAmount > 0)
        {
            var ar = Domain.ArTransaction.ArTransaction.Create(
                ArTransactionId.New(),
                Domain.ArTransaction.ArTransaction.SO_RETUR,
                retur.Id.Value,
                retur.ReturNumber,
                retur.TotalAmount,
                command.CreatedBy,
                dealerId);
            await arTransactionRepo.SaveAsync(ar, ct);
        }

        return retur.Id.Value;
    }
}
