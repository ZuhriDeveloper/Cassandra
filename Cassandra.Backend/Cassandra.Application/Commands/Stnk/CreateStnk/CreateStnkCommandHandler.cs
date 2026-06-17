using Cassandra.Application.Contracts.Bpkb;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.Contracts.Stnk;
using Cassandra.Domain.Common;
using DomainBpkb = Cassandra.Domain.Bpkb;
using DomainStnk = Cassandra.Domain.Stnk;

namespace Cassandra.Application.Commands.Stnk.CreateStnk;

public class CreateStnkCommandHandler(
    IStnkRepository               stnkRepository,
    IStnkQueryRepository          stnkQueryRepository,
    IBpkbRepository               bpkbRepository,
    IRegistrasiPenjualanQueryRepository registrasiQueryRepository,
    ICurrentDealer                currentDealer)
{
    public async Task<Guid> HandleAsync(CreateStnkCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        // 1. Validate RegistrasiPenjualan is SENT
        var registrasi = await registrasiQueryRepository.GetByIdAsync(command.RegistrasiPenjualanId, ct);
        if (registrasi is null)
            throw new DomainException($"Registrasi Penjualan dengan ID '{command.RegistrasiPenjualanId}' tidak ditemukan.");
        if (!registrasi.IsSent)
            throw new DomainException("STNK hanya dapat dibuat setelah kendaraan dikirim (status SENT).");

        // 2. Validate no STNK already exists for this RegistrasiPenjualanId
        if (await stnkQueryRepository.ExistsByRegistrasiPenjualanIdAsync(command.RegistrasiPenjualanId, ct))
            throw new DomainException($"STNK untuk Registrasi Penjualan ini sudah ada.");

        // 3. Create Stnk aggregate
        var stnk = DomainStnk.Stnk.Create(
            command.RegistrasiPenjualanId,
            command.FakturDate,
            command.FakturName,
            command.FakturAddress,
            command.CreatedBy,
            dealerId);

        await stnkRepository.SaveAsync(stnk, ct);

        // 4. Create Bpkb aggregate (auto-created simultaneously)
        var bpkb = DomainBpkb.Bpkb.Create(
            command.RegistrasiPenjualanId,
            stnk.Id.Value,
            command.FakturDate,
            command.CreatedBy,
            dealerId);

        await bpkbRepository.SaveAsync(bpkb, ct);

        return stnk.Id.Value;
    }
}
