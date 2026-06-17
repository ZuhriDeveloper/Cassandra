using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Application.Contracts.Kios;
using Cassandra.Application.Contracts.Mediator;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;
using Cassandra.Domain.Kios;
using Cassandra.Domain.Mediator;
using Cassandra.Domain.RegistrasiPenjualan;

namespace Cassandra.Application.Commands.RegistrasiPenjualan.ApproveRegistrasiPenjualan;

public class ApproveRegistrasiPenjualanCommandHandler(
    IRegistrasiPenjualanRepository     registrasiRepo,
    IRegistrasiPenjualanQueryRepository registrasiQueryRepo,
    IKaryawanRepository                karyawanRepo,
    IKaryawanQueryRepository           karyawanQueryRepo,
    IKiosRepository                    kiosRepo,
    IKiosQueryRepository               kiosQueryRepo,
    IMediatorRepository                mediatorRepo,
    IMediatorQueryRepository           mediatorQueryRepo)
{
    public async Task HandleAsync(ApproveRegistrasiPenjualanCommand command, CancellationToken ct = default)
    {
        var reg = await registrasiRepo.GetByIdAsync(RegistrasiPenjualanId.From(command.Id), ct)
            ?? throw new DomainException("Registrasi penjualan tidak ditemukan.");

        // Compute limit adjustment: restore delta from prev deduction
        decimal prevDeducted;
        decimal newDeducted;

        if (reg.MetodePenjualan == MetodePenjualanConstants.CASH)
        {
            prevDeducted = reg.Total;
            newDeducted  = reg.Total - command.ApprovedDiscount;
        }
        else
        {
            prevDeducted = Math.Max(0m, reg.Dp);
            newDeducted  = Math.Max(0m, reg.Dp - command.ApprovedDiscount);
        }

        decimal adjustment = prevDeducted - newDeducted; // positive = refund

        // Adjust Karyawan limit
        var karyawanDto = await karyawanQueryRepo.GetByIdAsync(reg.KaryawanId, ct);
        if (karyawanDto != null && adjustment != 0m)
        {
            var karyawan = await karyawanRepo.GetByIdAsync(KaryawanId.From(reg.KaryawanId), ct);
            if (karyawan != null)
            {
                karyawan.SetLimit(karyawanDto.SalesLimit + adjustment, "system");
                await karyawanRepo.SaveAsync(karyawan, ct);
            }
        }

        // Adjust Kios limit if KIOS
        if (reg.TipePenjualan == TipePenjualanConstants.KIOS && adjustment != 0m)
        {
            var kiosDto = await kiosQueryRepo.GetByIdAsync(reg.KiosId, ct);
            if (kiosDto != null)
            {
                var kios = await kiosRepo.GetByIdAsync(KiosId.From(reg.KiosId), ct);
                if (kios != null)
                {
                    kios.SetLimit(kiosDto.Limit + adjustment, "system");
                    await kiosRepo.SaveAsync(kios, ct);
                }
            }
        }

        // Adjust Mediator limit if MEDIATOR
        if (reg.TipePenjualan == TipePenjualanConstants.MEDIATOR && reg.MediatorId.HasValue && adjustment != 0m)
        {
            var mediatorDto = await mediatorQueryRepo.GetByIdAsync(reg.MediatorId.Value, ct);
            if (mediatorDto != null)
            {
                var mediator = await mediatorRepo.GetByIdAsync(MediatorId.From(reg.MediatorId.Value), ct);
                if (mediator != null)
                {
                    mediator.SetLimit(mediatorDto.Limit + adjustment, "system");
                    await mediatorRepo.SaveAsync(mediator, ct);
                }
            }
        }

        reg.Approve(command.ApprovedDiscount, command.ApprovedBy);
        await registrasiRepo.SaveAsync(reg, ct);
    }
}
