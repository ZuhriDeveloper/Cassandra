using Cassandra.Application.Contracts.CashOutTransaction;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Finance;
using Cassandra.Application.Contracts.So;
using Cassandra.Application.Contracts.SoPenerimaan;
using Cassandra.Application.Contracts.Stock;
using Cassandra.Domain.CashOutTransaction;
using Cassandra.Domain.Common;
using Cassandra.Domain.So;
using Cassandra.Domain.SoPenerimaan;
using Cassandra.Domain.Stock;
using Domain = Cassandra.Domain;

namespace Cassandra.Application.Commands.SoPenerimaan.CreateSoPenerimaan;

public class CreateSoPenerimaanCommandHandler(
    ISoPenerimaanRepository repository,
    ISoPenerimaanQueryRepository queryRepository,
    IStockRepository stockRepository,
    IStockQueryRepository stockQueryRepository,
    ISoQueryRepository soQueryRepository,
    ICashOutTransactionRepository cashOutRepo,
    IFinanceCounter financeCounter,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateSoPenerimaanCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var suratJalanId = command.SuratJalanId.Trim();

        // 1. Validate SuratJalanId unique
        if (await queryRepository.SuratJalanIdExistsAsync(suratJalanId, ct))
            throw new DomainException($"Nomor surat jalan '{suratJalanId}' sudah ada.");

        // 2. Validate each NoMesin is unique
        foreach (var motorItem in command.MotorItems)
        {
            if (await stockQueryRepository.NoMesinExistsAsync(motorItem.NoMesin.Trim(), ct))
                throw new DomainException($"Nomor mesin '{motorItem.NoMesin}' sudah terdaftar dalam stok.");
        }

        // 3. Validate SoId exists and is AKTIF
        if (!await soQueryRepository.IsSoAktifAsync(command.SoId, ct))
            throw new DomainException($"SO dengan ID '{command.SoId}' tidak ditemukan atau tidak berstatus AKTIF.");

        // 4. Create SOPenerimaan aggregate
        var motorValues = command.MotorItems
            .Select(m => new SoPenerimaanItemMotorValue(
                m.TipeMotorId, m.WarnaId, m.NoMesin.Trim(), m.NoRangka.Trim(), m.KiosId, m.AssemblyYear))
            .ToList();

        var kelengkapanValues = command.KelengkapanItems
            .Select(k => new SoPenerimaanItemKelengkapanValue(k.KelengkapanId, k.Qty, k.Notes))
            .ToList();

        var soPenerimaan = Domain.SoPenerimaan.SoPenerimaan.Create(
            suratJalanId,
            command.SuratJalanDate,
            command.SoId,
            motorValues,
            kelengkapanValues,
            command.CreatedBy,
            dealerId);

        // 5. Create Stock aggregates for each motor item
        foreach (var motorItem in motorValues)
        {
            var stock = Domain.Stock.Stock.Create(
                motorItem.NoMesin,
                motorItem.NoRangka,
                motorItem.TipeMotorId,
                motorItem.WarnaId,
                motorItem.KiosId,
                suratJalanId,
                command.SuratJalanDate,
                command.SoId,
                motorItem.AssemblyYear,
                command.CreatedBy,
                dealerId);

            await stockRepository.SaveAsync(stock, ct);
        }

        // 6. Save SOPenerimaan
        await repository.SaveAsync(soPenerimaan, ct);

        // 7. Create CashOut transaction for SO payment
        var soDto = await soQueryRepository.GetByIdAsync(command.SoId, ct);
        if (soDto is not null && soDto.TotalAmount > 0)
        {
            var transType = soDto.PaymentType == SoPaymentType.CASH
                ? Domain.CashOutTransaction.CashOutTransaction.FSO_CASH
                : Domain.CashOutTransaction.CashOutTransaction.FSO_DF;
            var fInvoiceId = await financeCounter.GetNextFInvoiceIdAsync(ct);
            var cashOut = Domain.CashOutTransaction.CashOutTransaction.Create(
                CashOutTransactionId.New(),
                transType,
                command.SoId,
                null,
                soDto.TotalAmount,
                0m,
                0,
                DateTime.UtcNow,
                "TRANSFER",
                fInvoiceId,
                command.CreatedBy,
                dealerId);
            await cashOutRepo.SaveAsync(cashOut, ct);
        }

        return soPenerimaan.Id.Value;
    }
}
