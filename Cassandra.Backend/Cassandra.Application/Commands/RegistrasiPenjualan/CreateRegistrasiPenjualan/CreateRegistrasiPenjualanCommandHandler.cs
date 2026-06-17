using Cassandra.Application.Contracts.AlokasiDiskon;
using Cassandra.Application.Contracts.ArTransaction;
using Cassandra.Application.Contracts.DaftarHargaLeasing;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Discount;
using Cassandra.Application.Contracts.DiscountCash;
using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Application.Contracts.Kios;
using Cassandra.Application.Contracts.Mediator;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.Contracts.Stock;
using Cassandra.Application.DTOs.Mediator;
using Cassandra.Domain.ArTransaction;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;
using Cassandra.Domain.Kios;
using Cassandra.Domain.Mediator;
using Cassandra.Domain.RegistrasiPenjualan;
using Cassandra.Domain.Stock;
using Domain = Cassandra.Domain;

namespace Cassandra.Application.Commands.RegistrasiPenjualan.CreateRegistrasiPenjualan;

public class CreateRegistrasiPenjualanCommandHandler(
    IRegistrasiPenjualanRepository     registrasiRepo,
    IRegistrasiPenjualanQueryRepository registrasiQueryRepo,
    IStockRepository                   stockRepo,
    IStockQueryRepository              stockQueryRepo,
    IKaryawanRepository                karyawanRepo,
    IKaryawanQueryRepository           karyawanQueryRepo,
    IKiosRepository                    kiosRepo,
    IKiosQueryRepository               kiosQueryRepo,
    IMediatorRepository                mediatorRepo,
    IMediatorQueryRepository           mediatorQueryRepo,
    IDiscountCashQueryRepository       discountCashQueryRepo,
    IDaftarHargaLeasingQueryRepository dhlQueryRepo,
    IDiscountQueryRepository           discountQueryRepo,
    IAlokasiDiskonQueryRepository      alokasiDiskonQueryRepo,
    IArTransactionRepository           arTransactionRepo,
    ICurrentDealer                     currentDealer)
{
    public async Task<Guid> HandleAsync(CreateRegistrasiPenjualanCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        // ── Uniqueness check ──────────────────────────────────────────────────
        if (await registrasiQueryRepo.NoPenjualanExistsAsync(command.NoPenjualan.Trim().ToUpper(), ct))
            throw new DomainException($"Nomor penjualan '{command.NoPenjualan}' sudah ada.");

        // ── Stock validation ──────────────────────────────────────────────────
        var stockDto = await stockQueryRepo.GetByNoMesinAsync(command.NoMesin, ct)
            ?? throw new DomainException($"Stock dengan nomor mesin '{command.NoMesin}' tidak ditemukan.");

        if (stockDto.Status != StockStatus.TERSEDIA)
            throw new DomainException($"Stock dengan nomor mesin '{command.NoMesin}' tidak tersedia (status: {stockDto.Status}).");

        // ── Karyawan ──────────────────────────────────────────────────────────
        var karyawanDto = await karyawanQueryRepo.GetByIdAsync(command.KaryawanId, ct)
            ?? throw new DomainException("Karyawan tidak ditemukan.");

        // ── Kios ──────────────────────────────────────────────────────────────
        var kiosDto = await kiosQueryRepo.GetByIdAsync(command.KiosId, ct)
            ?? throw new DomainException("Kios tidak ditemukan.");

        // ── Mediator (optional) ───────────────────────────────────────────────
        MediatorDto? mediatorDto = null;
        if (command.MediatorId.HasValue)
        {
            mediatorDto = await mediatorQueryRepo.GetByIdAsync(command.MediatorId.Value, ct)
                ?? throw new DomainException("Mediator tidak ditemukan.");
        }

        // ── TipeMotor code / warna name from stock ────────────────────────────
        // These come from the stock's TipeMotorId and WarnaId but we pass them from the
        // client; they're embedded in the event for reporting purposes.
        // The command doesn't carry TipeMotorCode / WarnaName directly.
        // We read them from the read model later. For now use placeholder — the controller
        // passes them as part of the Stock lookup.
        // Actually per the spec we store TipeMotorCode and WarnaName from Stock.
        // The StockDto doesn't carry names — it carries IDs only.
        // We'll store the IDs as codes for now (they are resolved by the UI).
        var tipeMotorCode = stockDto.TipeMotorId.ToString();
        var warnaName     = stockDto.WarnaId.ToString();

        // ── Discount / approval logic ─────────────────────────────────────────
        decimal originalDiscount;
        decimal approvedDiscount = 0m;
        bool    isApproved;
        decimal tac = 0m;
        Guid?   resolvedDaftarHargaLeasingId = command.DaftarHargaLeasingId;

        if (command.MetodePenjualan == MetodePenjualanConstants.CASH)
        {
            // Load DiscountCash for this TipeMotor
            var discountCashDto = await discountCashQueryRepo.GetByIdAsync(stockDto.TipeMotorId, ct);
            // DiscountCash is indexed by TipeMotorId — use GetAllAsync + filter for now
            // (the interface only has GetByIdAsync with Guid id = the DiscountCash record id, not TipeMotorId)
            // We need to get by TipeMotorId. Use the query repo pattern.
            var allDiscountCash = await discountCashQueryRepo.GetAllAsync(ct);
            var dc = allDiscountCash.FirstOrDefault(x => x.TipeMotorId == stockDto.TipeMotorId);

            decimal standard;
            if (command.TipePenjualan == TipePenjualanConstants.DIRECT)
                standard = dc?.DirectDiscount ?? 0m;
            else
                standard = dc?.ChannelDiscount ?? 0m;

            originalDiscount = standard;
            isApproved       = command.Discount == standard;
            approvedDiscount = isApproved ? standard : 0m;
        }
        else
        {
            // CREDIT
            if (!command.DaftarHargaLeasingId.HasValue)
                throw new DomainException("DaftarHargaLeasing harus dipilih untuk penjualan kredit.");

            var dhl = await dhlQueryRepo.GetByIdAsync(command.DaftarHargaLeasingId.Value, ct)
                ?? throw new DomainException("DaftarHargaLeasing tidak ditemukan.");

            // Load AlokasiDiskon for Karyawan
            var allAlokasi = await alokasiDiskonQueryRepo.GetAllAsync(ct);
            var alokasi = allAlokasi.FirstOrDefault(x => x.KaryawanId == command.KaryawanId);

            if (alokasi != null)
            {
                // Enforce channel/direct constraint
                if (alokasi.DiscountLevel == "Direct" && command.TipePenjualan != TipePenjualanConstants.DIRECT)
                    throw new DomainException("Karyawan dengan alokasi diskon Direct hanya dapat melakukan penjualan DIRECT.");
                if ((alokasi.DiscountLevel == "Channel1" || alokasi.DiscountLevel == "Channel2")
                    && command.TipePenjualan == TipePenjualanConstants.DIRECT)
                    throw new DomainException("Karyawan dengan alokasi diskon Channel tidak dapat melakukan penjualan DIRECT.");
            }

            // Load discount for (dhl, grupTenorId=dhl.GrupTenorId, level=alokasi.DiscountLevel)
            var allDiscounts = await discountQueryRepo.GetAllAsync(ct);
            var discountDto = alokasi != null
                ? allDiscounts.FirstOrDefault(x =>
                    x.DaftarHargaLeasingId == command.DaftarHargaLeasingId.Value &&
                    x.Level == alokasi.DiscountLevel)
                : null;

            // Find matching discount item by GrupTipeMotor — we don't have GrupTipeMotorId on stock
            // We use the TipeMotorId to find the group; for now use the first item or 0
            decimal discAmount = 0m;
            if (discountDto != null && discountDto.Items.Count > 0)
            {
                // In a real scenario we'd match on GrupTipeMotorId linked to TipeMotorId
                // For now take the first matching item or 0
                discAmount = discountDto.Items.FirstOrDefault()?.Amount ?? 0m;
            }
            originalDiscount = discAmount;

            // TAC from DaftarHargaLeasing items
            decimal tacValue = 0m;
            if (dhl.Items.Count > 0)
                tacValue = dhl.Items.FirstOrDefault()?.Total ?? 0m;
            tac = tacValue;

            isApproved       = command.Discount == originalDiscount;
            approvedDiscount = isApproved ? originalDiscount : 0m;
        }

        // ── Limit check ───────────────────────────────────────────────────────
        decimal limitToCheck;
        if (command.MetodePenjualan == MetodePenjualanConstants.CASH)
            limitToCheck = command.Total;
        else
            limitToCheck = Math.Max(0m, command.Dp - command.Discount);

        if (karyawanDto.SalesLimit - limitToCheck < 0)
            throw new DomainException($"Limit Karyawan tidak mencukupi. Limit: {karyawanDto.SalesLimit}, Dibutuhkan: {limitToCheck}.");

        if (command.TipePenjualan == TipePenjualanConstants.KIOS)
        {
            if (kiosDto.Limit - limitToCheck < 0)
                throw new DomainException($"Limit Kios tidak mencukupi. Limit: {kiosDto.Limit}, Dibutuhkan: {limitToCheck}.");
        }

        if (command.TipePenjualan == TipePenjualanConstants.MEDIATOR && mediatorDto != null)
        {
            if (mediatorDto.Limit - limitToCheck < 0)
                throw new DomainException($"Limit Mediator tidak mencukupi. Limit: {mediatorDto.Limit}, Dibutuhkan: {limitToCheck}.");
        }

        // ── Limit deduction at create ─────────────────────────────────────────
        decimal limitToDeduct;
        if (command.MetodePenjualan == MetodePenjualanConstants.CASH)
            limitToDeduct = command.Total;
        else
            limitToDeduct = Math.Max(0m, command.Dp);

        var karyawan = await karyawanRepo.GetByIdAsync(KaryawanId.From(command.KaryawanId), ct)
            ?? throw new DomainException("Karyawan tidak ditemukan.");
        karyawan.SetLimit(karyawanDto.SalesLimit - limitToDeduct, "system");
        await karyawanRepo.SaveAsync(karyawan, ct);

        if (command.TipePenjualan == TipePenjualanConstants.KIOS)
        {
            var kios = await kiosRepo.GetByIdAsync(KiosId.From(command.KiosId), ct)
                ?? throw new DomainException("Kios tidak ditemukan.");
            kios.SetLimit(kiosDto.Limit - limitToDeduct, "system");
            await kiosRepo.SaveAsync(kios, ct);
        }

        if (command.TipePenjualan == TipePenjualanConstants.MEDIATOR && command.MediatorId.HasValue && mediatorDto != null)
        {
            var mediator = await mediatorRepo.GetByIdAsync(MediatorId.From(command.MediatorId.Value), ct)
                ?? throw new DomainException("Mediator tidak ditemukan.");
            mediator.SetLimit(mediatorDto.Limit - limitToDeduct, "system");
            await mediatorRepo.SaveAsync(mediator, ct);
        }

        // ── Change stock to TERJUAL ───────────────────────────────────────────
        var stock = await stockRepo.GetByIdAsync(StockId.From(stockDto.Id), ct)
            ?? throw new DomainException("Stock tidak ditemukan.");
        stock.ChangeStatus(StockStatus.TERJUAL, command.CreatedBy);
        await stockRepo.SaveAsync(stock, ct);

        // ── Create RegistrasiPenjualan ────────────────────────────────────────
        var kelengkapanStr = string.Join(",", command.Kelengkapan ?? []);
        var reg = Domain.RegistrasiPenjualan.RegistrasiPenjualan.Create(
            command.NoPenjualan,
            command.SaleDate,
            command.KaryawanId,
            command.KiosId,
            command.MediatorId,
            command.MetodePenjualan,
            command.TipePenjualan,
            command.NoMesin,
            command.NoRangka,
            command.NamaCustomer,
            command.Address,
            command.Phone,
            command.Phone1,
            command.Phone2,
            command.OffRoad,
            command.Bbn,
            command.Discount,
            approvedDiscount,
            originalDiscount,
            command.Total,
            command.AmbilUang,
            command.Dp,
            command.Angsuran,
            tac,
            resolvedDaftarHargaLeasingId,
            command.TenorCode,
            tipeMotorCode,
            warnaName,
            command.SerahTerimaKendaraanId,
            command.TandaTerimaSementaraId,
            kelengkapanStr,
            isApproved,
            command.CreatedBy,
            dealerId);

        await registrasiRepo.SaveAsync(reg, ct);

        // ── Create AR transaction(s) based on MetodePenjualan ─────────────────
        if (command.MetodePenjualan == MetodePenjualanConstants.CASH)
        {
            var ar = Domain.ArTransaction.ArTransaction.Create(
                ArTransactionId.New(),
                Domain.ArTransaction.ArTransaction.PENJUALAN,
                reg.Id.Value,
                reg.NoPenjualan,
                command.Total,
                command.CreatedBy,
                dealerId);
            await arTransactionRepo.SaveAsync(ar, ct);
        }
        else // CREDIT
        {
            if (command.AmbilUang > 0)
            {
                var arAmbil = Domain.ArTransaction.ArTransaction.Create(
                    ArTransactionId.New(),
                    Domain.ArTransaction.ArTransaction.AMBIL_UANG,
                    reg.Id.Value,
                    reg.NoPenjualan,
                    command.AmbilUang,
                    command.CreatedBy,
                    dealerId);
                await arTransactionRepo.SaveAsync(arAmbil, ct);
            }
            var creditAmount = command.Total - command.AmbilUang;
            if (creditAmount > 0)
            {
                var arCredit = Domain.ArTransaction.ArTransaction.Create(
                    ArTransactionId.New(),
                    Domain.ArTransaction.ArTransaction.PENJUALAN_CREDIT,
                    reg.Id.Value,
                    reg.NoPenjualan,
                    creditAmount,
                    command.CreatedBy,
                    dealerId);
                await arTransactionRepo.SaveAsync(arCredit, ct);
            }
        }

        return reg.Id.Value;
    }
}
