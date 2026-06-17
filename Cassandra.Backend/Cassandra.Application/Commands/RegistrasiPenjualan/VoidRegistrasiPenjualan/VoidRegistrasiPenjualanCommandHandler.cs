using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Application.Contracts.Kios;
using Cassandra.Application.Contracts.Mediator;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.Contracts.Stock;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;
using Cassandra.Domain.Kios;
using Cassandra.Domain.Mediator;
using Cassandra.Domain.RegistrasiPenjualan;
using Cassandra.Domain.Stock;

namespace Cassandra.Application.Commands.RegistrasiPenjualan.VoidRegistrasiPenjualan;

public class VoidRegistrasiPenjualanCommandHandler(
    IRegistrasiPenjualanRepository     registrasiRepo,
    IStockRepository                   stockRepo,
    IStockQueryRepository              stockQueryRepo,
    IKaryawanRepository                karyawanRepo,
    IKaryawanQueryRepository           karyawanQueryRepo,
    IKiosRepository                    kiosRepo,
    IKiosQueryRepository               kiosQueryRepo,
    IMediatorRepository                mediatorRepo,
    IMediatorQueryRepository           mediatorQueryRepo)
{
    public async Task HandleAsync(VoidRegistrasiPenjualanCommand command, CancellationToken ct = default)
    {
        var reg = await registrasiRepo.GetByIdAsync(RegistrasiPenjualanId.From(command.Id), ct)
            ?? throw new DomainException("Registrasi penjualan tidak ditemukan.");

        // Restore limit: Total - ApprovedDiscount (for both CASH and CREDIT on void)
        decimal restoreAmount = reg.Total - reg.ApprovedDiscount;

        // Restore Stock to TERSEDIA
        var stockDto = await stockQueryRepo.GetByNoMesinAsync(reg.NoMesin, ct);
        if (stockDto != null)
        {
            var stock = await stockRepo.GetByIdAsync(StockId.From(stockDto.Id), ct);
            if (stock != null)
            {
                stock.ChangeStatus(StockStatus.TERSEDIA, command.VoidedBy);
                await stockRepo.SaveAsync(stock, ct);
            }
        }

        // Restore Karyawan limit
        var karyawanDto = await karyawanQueryRepo.GetByIdAsync(reg.KaryawanId, ct);
        if (karyawanDto != null)
        {
            var karyawan = await karyawanRepo.GetByIdAsync(KaryawanId.From(reg.KaryawanId), ct);
            if (karyawan != null)
            {
                karyawan.SetLimit(karyawanDto.SalesLimit + restoreAmount, "system");
                await karyawanRepo.SaveAsync(karyawan, ct);
            }
        }

        // Restore Kios limit if KIOS
        if (reg.TipePenjualan == TipePenjualanConstants.KIOS)
        {
            var kiosDto = await kiosQueryRepo.GetByIdAsync(reg.KiosId, ct);
            if (kiosDto != null)
            {
                var kios = await kiosRepo.GetByIdAsync(KiosId.From(reg.KiosId), ct);
                if (kios != null)
                {
                    kios.SetLimit(kiosDto.Limit + restoreAmount, "system");
                    await kiosRepo.SaveAsync(kios, ct);
                }
            }
        }

        // Restore Mediator limit if MEDIATOR
        if (reg.TipePenjualan == TipePenjualanConstants.MEDIATOR && reg.MediatorId.HasValue)
        {
            var mediatorDto = await mediatorQueryRepo.GetByIdAsync(reg.MediatorId.Value, ct);
            if (mediatorDto != null)
            {
                var mediator = await mediatorRepo.GetByIdAsync(MediatorId.From(reg.MediatorId.Value), ct);
                if (mediator != null)
                {
                    mediator.SetLimit(mediatorDto.Limit + restoreAmount, "system");
                    await mediatorRepo.SaveAsync(mediator, ct);
                }
            }
        }

        reg.Void(command.VoidedBy);
        await registrasiRepo.SaveAsync(reg, ct);
    }
}
