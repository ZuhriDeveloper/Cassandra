using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.PengirimanMotor;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.Contracts.Stock;
using Cassandra.Domain.Common;
using Cassandra.Domain.PengirimanMotor;
using Cassandra.Domain.RegistrasiPenjualan;
using Cassandra.Domain.Stock;

namespace Cassandra.Application.Commands.PengirimanMotor.CreatePengirimanMotor;

public class CreatePengirimanMotorCommandHandler(
    IPengirimanMotorRepository          pengirimanRepo,
    IRegistrasiPenjualanRepository      registrasiRepo,
    IRegistrasiPenjualanQueryRepository registrasiQueryRepo,
    IStockRepository                    stockRepo,
    IStockQueryRepository               stockQueryRepo,
    ICurrentDealer                      currentDealer)
{
    public async Task<Guid> HandleAsync(CreatePengirimanMotorCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        // ── Validate RegistrasiPenjualan ──────────────────────────────────────
        var registrasiDto = await registrasiQueryRepo.GetByIdAsync(command.RegistrasiPenjualanId, ct)
            ?? throw new DomainException("Registrasi penjualan tidak ditemukan.");

        if (registrasiDto.IsVoid)
            throw new DomainException("Registrasi penjualan sudah di-void.");
        if (!registrasiDto.IsApproved)
            throw new DomainException("Registrasi penjualan belum disetujui.");
        if (registrasiDto.IsSent)
            throw new DomainException("Registrasi penjualan sudah dikirim.");

        // ── Validate Stock status ─────────────────────────────────────────────
        var stockDto = await stockQueryRepo.GetByNoMesinAsync(command.NoMesin, ct)
            ?? throw new DomainException($"Stock dengan nomor mesin '{command.NoMesin}' tidak ditemukan.");

        if (stockDto.Status != StockStatus.TERJUAL)
            throw new DomainException($"Stock harus berstatus TERJUAL untuk dibuat pengiriman (status: {stockDto.Status}).");

        // ── Create PengirimanMotor ────────────────────────────────────────────
        var pengiriman = Domain.PengirimanMotor.PengirimanMotor.Create(
            command.RegistrasiPenjualanId,
            command.NoMesin,
            command.Driver1Id,
            command.Driver2Id,
            command.DeliveryDate,
            command.Zona,
            command.CreatedBy,
            dealerId);

        await pengirimanRepo.SaveAsync(pengiriman, ct);

        // ── Change Stock to TERKIRIM ──────────────────────────────────────────
        var stock = await stockRepo.GetByIdAsync(StockId.From(stockDto.Id), ct)
            ?? throw new DomainException("Stock tidak ditemukan.");
        stock.ChangeStatus(StockStatus.TERKIRIM, command.CreatedBy);
        await stockRepo.SaveAsync(stock, ct);

        // ── Mark RegistrasiPenjualan as Sent ──────────────────────────────────
        var reg = await registrasiRepo.GetByIdAsync(RegistrasiPenjualanId.From(command.RegistrasiPenjualanId), ct)
            ?? throw new DomainException("Registrasi penjualan tidak ditemukan.");
        reg.MarkAsSent(command.CreatedBy);
        await registrasiRepo.SaveAsync(reg, ct);

        return pengiriman.Id.Value;
    }
}
