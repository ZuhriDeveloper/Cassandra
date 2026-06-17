using Cassandra.Application.Commands.ApTransaction.CreateApTransaction;
using Cassandra.Application.Commands.ApTransaction.RecordApPayment;
using Cassandra.Application.Commands.ArTransaction.CreateArTransaction;
using Cassandra.Application.Commands.ArTransaction.RecordArPayment;
using Cassandra.Application.Commands.Bpkb.HandoverBpkb;
using Cassandra.Application.Commands.CashOutTransaction.CreateCashOutTransaction;
using Cassandra.Application.Queries.ApTransaction;
using Cassandra.Application.Queries.ArTransaction;
using Cassandra.Application.Queries.CashOutTransaction;
using Cassandra.Application.Queries.Finance;
using Cassandra.Application.Commands.Bpkb.ReceiveBpkb;
using Cassandra.Application.Commands.Stnk.CreateStnk;
using Cassandra.Application.Commands.Stnk.HandoverStnk;
using Cassandra.Application.Commands.Stnk.ProcessStnk;
using Cassandra.Application.Commands.Stnk.ReceiveStnk;
using Cassandra.Application.Queries.Bpkb;
using Cassandra.Application.Queries.Stnk;
using Cassandra.Application.Commands.Mutasi.CreateMutasi;
using Cassandra.Application.Commands.PengirimanMotor.CreatePengirimanMotor;
using Cassandra.Application.Commands.RegistrasiPenjualan.ApproveRegistrasiPenjualan;
using Cassandra.Application.Commands.RegistrasiPenjualan.CreateRegistrasiPenjualan;
using Cassandra.Application.Commands.RegistrasiPenjualan.SetEnableToVoid;
using Cassandra.Application.Commands.RegistrasiPenjualan.VoidRegistrasiPenjualan;
using Cassandra.Application.Queries.PengirimanMotor;
using Cassandra.Application.Queries.RegistrasiPenjualan;
using Cassandra.Application.Commands.So.ChangeSoStatus;
using Cassandra.Application.Commands.So.CreateSo;
using Cassandra.Application.Commands.So.DeleteSo;
using Cassandra.Application.Commands.SoPenerimaan.CreateSoPenerimaan;
using Cassandra.Application.Commands.SoRetur.CreateSoRetur;
using Cassandra.Application.Commands.Stock.ChangeStockStatus;
using Cassandra.Application.Queries.Mutasi;
using Cassandra.Application.Queries.So;
using Cassandra.Application.Queries.SoPenerimaan;
using Cassandra.Application.Queries.SoRetur;
using Cassandra.Application.Queries.Stock;
using Cassandra.Application.Commands.AlokasiDiskon.CreateAlokasiDiskon;
using Cassandra.Application.Commands.BiayaBiroJasa.CreateBiayaBiroJasa;
using Cassandra.Application.Commands.BiayaBiroJasa.SetBiayaBiroJasaItems;
using Cassandra.Application.Commands.BiayaBiroJasa.SetBiayaBiroJasaStatus;
using Cassandra.Application.Commands.Biro.CreateBiro;
using Cassandra.Application.Commands.Biro.SetBiroStatus;
using Cassandra.Application.Commands.Biro.UpdateBiro;
using Cassandra.Application.Commands.ExpenseType.CreateExpenseType;
using Cassandra.Application.Commands.ExpenseType.SetExpenseTypeStatus;
using Cassandra.Application.Commands.ExpenseType.UpdateExpenseType;
using Cassandra.Application.Commands.Ledger.CreateLedger;
using Cassandra.Application.Commands.Ledger.SetLedgerStatus;
using Cassandra.Application.Commands.Ledger.UpdateLedger;
using Cassandra.Application.Commands.PelanggaranWilayah.CreatePelanggaranWilayah;
using Cassandra.Application.Commands.PelanggaranWilayah.SetPelanggaranWilayahStatus;
using Cassandra.Application.Commands.PelanggaranWilayah.UpdatePelanggaranWilayah;
using Cassandra.Application.Commands.Samsat.CreateSamsat;
using Cassandra.Application.Commands.Samsat.SetSamsatCities;
using Cassandra.Application.Commands.Samsat.SetSamsatStatus;
using Cassandra.Application.Commands.Samsat.UpdateSamsat;
using Cassandra.Application.Queries.BiayaBiroJasa;
using Cassandra.Application.Queries.Biro;
using Cassandra.Application.Queries.ExpenseType;
using Cassandra.Application.Queries.Ledger;
using Cassandra.Application.Queries.PelanggaranWilayah;
using Cassandra.Application.Queries.Samsat;
using Cassandra.Application.Commands.AlokasiDiskon.SetAlokasiDiskonStatus;
using Cassandra.Application.Commands.AlokasiDiskon.UpdateAlokasiDiskon;
using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Commands.CabangLeasing.CreateCabangLeasing;
using Cassandra.Application.Commands.CabangLeasing.SetCabangLeasingStatus;
using Cassandra.Application.Commands.CabangLeasing.UpdateCabangLeasing;
using Cassandra.Application.Commands.DaftarHargaLeasing.CreateDaftarHargaLeasing;
using Cassandra.Application.Commands.DaftarHargaLeasing.SetDaftarHargaLeasingItems;
using Cassandra.Application.Commands.DaftarHargaLeasing.SetDaftarHargaLeasingStatus;
using Cassandra.Application.Commands.DaftarHargaLeasing.UpdateDaftarHargaLeasing;
using Cassandra.Application.Commands.Dealers.RegisterDealer;
using Cassandra.Application.Commands.Dealers.RenameDealer;
using Cassandra.Application.Commands.Dealers.SetDealerStatus;
using Cassandra.Application.Commands.Df.SetDf;
using Cassandra.Application.Commands.Discount.CreateDiscount;
using Cassandra.Application.Commands.Discount.SetDiscountItems;
using Cassandra.Application.Commands.Discount.SetDiscountStatus;
using Cassandra.Application.Commands.Discount.UpdateDiscount;
using Cassandra.Application.Commands.DiscountCash.CreateDiscountCash;
using Cassandra.Application.Commands.DiscountCash.SetDiscountCashStatus;
using Cassandra.Application.Commands.DiscountCash.UpdateDiscountCash;
using Cassandra.Application.Commands.GlobalLeasing.CreateGlobalLeasing;
using Cassandra.Application.Commands.GlobalLeasing.SetGlobalLeasingStatus;
using Cassandra.Application.Commands.GlobalLeasing.UpdateGlobalLeasing;
using Cassandra.Application.Commands.GrupTenor.CreateGrupTenor;
using Cassandra.Application.Commands.GrupTenor.SetGrupTenorStatus;
using Cassandra.Application.Commands.GrupTenor.UpdateGrupTenor;
using Cassandra.Application.Commands.GrupTipeMotor.CreateGrupTipeMotor;
using Cassandra.Application.Commands.GrupTipeMotor.SetGrupTipeMotorStatus;
using Cassandra.Application.Commands.Jabatan.CreateJabatan;
using Cassandra.Application.Commands.Jabatan.SetJabatanStatus;
using Cassandra.Application.Commands.Jabatan.UpdateJabatan;
using Cassandra.Application.Commands.Karyawan.CreateKaryawan;
using Cassandra.Application.Commands.Karyawan.RecordKaryawanResign;
using Cassandra.Application.Commands.Karyawan.SetKaryawanLimit;
using Cassandra.Application.Commands.Karyawan.SetKaryawanStatus;
using Cassandra.Application.Commands.Karyawan.UpdateKaryawan;
using Cassandra.Application.Commands.Kelengkapan.CreateKelengkapan;
using Cassandra.Application.Commands.Kelengkapan.SetKelengkapanStatus;
using Cassandra.Application.Commands.Kelengkapan.UpdateKelengkapan;
using Cassandra.Application.Commands.Kios.CreateKios;
using Cassandra.Application.Commands.Kios.SetKiosLimit;
using Cassandra.Application.Commands.Kios.SetKiosStatus;
using Cassandra.Application.Commands.Kios.UpdateKios;
using Cassandra.Application.Commands.Mediator.CreateMediator;
using Cassandra.Application.Commands.Mediator.SetMediatorLimit;
using Cassandra.Application.Commands.Mediator.SetMediatorStatus;
using Cassandra.Application.Commands.Mediator.UpdateMediator;
using Cassandra.Application.Commands.MetodeKeuangan.CreateMetodeKeuangan;
using Cassandra.Application.Commands.MetodeKeuangan.SetMetodeKeuanganStatus;
using Cassandra.Application.Commands.MetodeKeuangan.UpdateMetodeKeuangan;
using Cassandra.Application.Commands.Tenor.CreateTenor;
using Cassandra.Application.Commands.Tenor.SetTenorStatus;
using Cassandra.Application.Commands.Tenor.UpdateTenor;
using Cassandra.Application.Commands.TipeMotor.CreateTipeMotor;
using Cassandra.Application.Commands.TipeMotor.SetTipeMotorColors;
using Cassandra.Application.Commands.TipeMotor.SetTipeMotorStatus;
using Cassandra.Application.Commands.TipeMotor.UpdateTipeMotor;
using Cassandra.Application.Commands.Warna.CreateWarna;
using Cassandra.Application.Commands.Warna.SetWarnaStatus;
using Cassandra.Application.Commands.Warna.UpdateWarna;
using Cassandra.Application.Queries.AlokasiDiskon;
using Cassandra.Application.Queries.CabangLeasing;
using Cassandra.Application.Queries.DaftarHargaLeasing;
using Cassandra.Application.Queries.Dealers;
using Cassandra.Application.Queries.Df;
using Cassandra.Application.Queries.Discount;
using Cassandra.Application.Queries.DiscountCash;
using Cassandra.Application.Queries.GlobalLeasing;
using Cassandra.Application.Queries.GrupTenor;
using Cassandra.Application.Queries.GrupTipeMotor;
using Cassandra.Application.Queries.Jabatan;
using Cassandra.Application.Queries.Karyawan;
using Cassandra.Application.Queries.Kelengkapan;
using Cassandra.Application.Queries.Kios;
using Cassandra.Application.Queries.Mediator;
using Cassandra.Application.Queries.MetodeKeuangan;
using Cassandra.Application.Queries.Tenor;
using Cassandra.Application.Queries.TipeMotor;
using Cassandra.Application.Queries.Warna;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Cassandra.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Auth
        services.AddScoped<LoginCommandHandler>();

        // Dealers
        services.AddScoped<RegisterDealerCommandHandler>();
        services.AddScoped<RenameDealerCommandHandler>();
        services.AddScoped<SetDealerStatusCommandHandler>();
        services.AddScoped<GetDealersQueryHandler>();

        // Jabatan
        services.AddScoped<CreateJabatanCommandHandler>();
        services.AddScoped<UpdateJabatanCommandHandler>();
        services.AddScoped<SetJabatanStatusCommandHandler>();
        services.AddScoped<GetJabatansQueryHandler>();

        // Karyawan
        services.AddScoped<CreateKaryawanCommandHandler>();
        services.AddScoped<UpdateKaryawanCommandHandler>();
        services.AddScoped<SetKaryawanStatusCommandHandler>();
        services.AddScoped<SetKaryawanLimitCommandHandler>();
        services.AddScoped<RecordKaryawanResignCommandHandler>();
        services.AddScoped<GetKaryawansQueryHandler>();

        // Kios
        services.AddScoped<CreateKiosCommandHandler>();
        services.AddScoped<UpdateKiosCommandHandler>();
        services.AddScoped<SetKiosStatusCommandHandler>();
        services.AddScoped<SetKiosLimitCommandHandler>();
        services.AddScoped<GetKiosQueryHandler>();

        // Mediator
        services.AddScoped<CreateMediatorCommandHandler>();
        services.AddScoped<UpdateMediatorCommandHandler>();
        services.AddScoped<SetMediatorStatusCommandHandler>();
        services.AddScoped<SetMediatorLimitCommandHandler>();
        services.AddScoped<GetMediatorsQueryHandler>();

        // Warna
        services.AddScoped<CreateWarnaCommandHandler>();
        services.AddScoped<UpdateWarnaCommandHandler>();
        services.AddScoped<SetWarnaStatusCommandHandler>();
        services.AddScoped<GetWarnasQueryHandler>();

        // GrupTipeMotor
        services.AddScoped<CreateGrupTipeMotorCommandHandler>();
        services.AddScoped<SetGrupTipeMotorStatusCommandHandler>();
        services.AddScoped<GetGrupTipeMotorsQueryHandler>();

        // TipeMotor
        services.AddScoped<CreateTipeMotorCommandHandler>();
        services.AddScoped<UpdateTipeMotorCommandHandler>();
        services.AddScoped<SetTipeMotorStatusCommandHandler>();
        services.AddScoped<SetTipeMotorColorsCommandHandler>();
        services.AddScoped<GetTipeMotorsQueryHandler>();

        // Kelengkapan
        services.AddScoped<CreateKelengkapanCommandHandler>();
        services.AddScoped<UpdateKelengkapanCommandHandler>();
        services.AddScoped<SetKelengkapanStatusCommandHandler>();
        services.AddScoped<GetKelengkapansQueryHandler>();

        // MetodeKeuangan
        services.AddScoped<CreateMetodeKeuanganCommandHandler>();
        services.AddScoped<UpdateMetodeKeuanganCommandHandler>();
        services.AddScoped<SetMetodeKeuanganStatusCommandHandler>();
        services.AddScoped<GetMetodeKeuangansQueryHandler>();

        // GlobalLeasing
        services.AddScoped<CreateGlobalLeasingCommandHandler>();
        services.AddScoped<UpdateGlobalLeasingCommandHandler>();
        services.AddScoped<SetGlobalLeasingStatusCommandHandler>();
        services.AddScoped<GetGlobalLeasingsQueryHandler>();

        // CabangLeasing
        services.AddScoped<CreateCabangLeasingCommandHandler>();
        services.AddScoped<UpdateCabangLeasingCommandHandler>();
        services.AddScoped<SetCabangLeasingStatusCommandHandler>();
        services.AddScoped<GetCabangLeasingsQueryHandler>();

        // GrupTenor
        services.AddScoped<CreateGrupTenorCommandHandler>();
        services.AddScoped<UpdateGrupTenorCommandHandler>();
        services.AddScoped<SetGrupTenorStatusCommandHandler>();
        services.AddScoped<GetGrupTenorsQueryHandler>();

        // Tenor
        services.AddScoped<CreateTenorCommandHandler>();
        services.AddScoped<UpdateTenorCommandHandler>();
        services.AddScoped<SetTenorStatusCommandHandler>();
        services.AddScoped<GetTenorsQueryHandler>();

        // Df
        services.AddScoped<SetDfCommandHandler>();
        services.AddScoped<GetDfQueryHandler>();

        // DaftarHargaLeasing
        services.AddScoped<CreateDaftarHargaLeasingCommandHandler>();
        services.AddScoped<UpdateDaftarHargaLeasingCommandHandler>();
        services.AddScoped<SetDaftarHargaLeasingStatusCommandHandler>();
        services.AddScoped<SetDaftarHargaLeasingItemsCommandHandler>();
        services.AddScoped<GetDaftarHargaLeasingsQueryHandler>();

        // Discount
        services.AddScoped<CreateDiscountCommandHandler>();
        services.AddScoped<UpdateDiscountCommandHandler>();
        services.AddScoped<SetDiscountStatusCommandHandler>();
        services.AddScoped<SetDiscountItemsCommandHandler>();
        services.AddScoped<GetDiscountsQueryHandler>();

        // DiscountCash
        services.AddScoped<CreateDiscountCashCommandHandler>();
        services.AddScoped<UpdateDiscountCashCommandHandler>();
        services.AddScoped<SetDiscountCashStatusCommandHandler>();
        services.AddScoped<GetDiscountCashsQueryHandler>();

        // AlokasiDiskon
        services.AddScoped<CreateAlokasiDiskonCommandHandler>();
        services.AddScoped<UpdateAlokasiDiskonCommandHandler>();
        services.AddScoped<SetAlokasiDiskonStatusCommandHandler>();
        services.AddScoped<GetAlokasiDiskonsQueryHandler>();

        // Phase 4: Service Bureau & Finance Config

        // Samsat
        services.AddScoped<CreateSamsatCommandHandler>();
        services.AddScoped<UpdateSamsatCommandHandler>();
        services.AddScoped<SetSamsatStatusCommandHandler>();
        services.AddScoped<SetSamsatCitiesCommandHandler>();
        services.AddScoped<GetSamsatsQueryHandler>();

        // Biro
        services.AddScoped<CreateBiroCommandHandler>();
        services.AddScoped<UpdateBiroCommandHandler>();
        services.AddScoped<SetBiroStatusCommandHandler>();
        services.AddScoped<GetBirosQueryHandler>();

        // BiayaBiroJasa
        services.AddScoped<CreateBiayaBiroJasaCommandHandler>();
        services.AddScoped<SetBiayaBiroJasaStatusCommandHandler>();
        services.AddScoped<SetBiayaBiroJasaItemsCommandHandler>();
        services.AddScoped<GetBiayaBiroJasasQueryHandler>();

        // ExpenseType
        services.AddScoped<CreateExpenseTypeCommandHandler>();
        services.AddScoped<UpdateExpenseTypeCommandHandler>();
        services.AddScoped<SetExpenseTypeStatusCommandHandler>();
        services.AddScoped<GetExpenseTypesQueryHandler>();

        // Ledger
        services.AddScoped<CreateLedgerCommandHandler>();
        services.AddScoped<UpdateLedgerCommandHandler>();
        services.AddScoped<SetLedgerStatusCommandHandler>();
        services.AddScoped<GetLedgersQueryHandler>();

        // PelanggaranWilayah
        services.AddScoped<CreatePelanggaranWilayahCommandHandler>();
        services.AddScoped<UpdatePelanggaranWilayahCommandHandler>();
        services.AddScoped<SetPelanggaranWilayahStatusCommandHandler>();
        services.AddScoped<GetPelanggaranWilayahsQueryHandler>();

        // Phase 5: Inventory & Stock

        // So
        services.AddScoped<CreateSoCommandHandler>();
        services.AddScoped<ChangeSoStatusCommandHandler>();
        services.AddScoped<DeleteSoCommandHandler>();
        services.AddScoped<GetSosQueryHandler>();

        // Stock
        services.AddScoped<ChangeStockStatusCommandHandler>();
        services.AddScoped<GetStocksQueryHandler>();

        // SoPenerimaan
        services.AddScoped<CreateSoPenerimaanCommandHandler>();
        services.AddScoped<GetSoPenerimaansQueryHandler>();

        // SoRetur
        services.AddScoped<CreateSoReturCommandHandler>();
        services.AddScoped<GetSoRetursQueryHandler>();

        // Mutasi
        services.AddScoped<CreateMutasiCommandHandler>();
        services.AddScoped<GetMutasisQueryHandler>();

        // Phase 6: Sales

        // RegistrasiPenjualan
        services.AddScoped<CreateRegistrasiPenjualanCommandHandler>();
        services.AddScoped<ApproveRegistrasiPenjualanCommandHandler>();
        services.AddScoped<VoidRegistrasiPenjualanCommandHandler>();
        services.AddScoped<SetEnableToVoidCommandHandler>();
        services.AddScoped<GetRegistrasiPenjualansQueryHandler>();

        // PengirimanMotor
        services.AddScoped<CreatePengirimanMotorCommandHandler>();
        services.AddScoped<GetPengirimanMotorsQueryHandler>();

        // Phase 7: Document Workflows

        // Stnk
        services.AddScoped<CreateStnkCommandHandler>();
        services.AddScoped<ProcessStnkCommandHandler>();
        services.AddScoped<ReceiveStnkCommandHandler>();
        services.AddScoped<HandoverStnkCommandHandler>();
        services.AddScoped<GetStnksQueryHandler>();

        // Bpkb
        services.AddScoped<ReceiveBpkbCommandHandler>();
        services.AddScoped<HandoverBpkbCommandHandler>();
        services.AddScoped<GetBpkbsQueryHandler>();

        // Phase 8: Finance & Accounting

        // ArTransaction
        services.AddScoped<CreateArTransactionCommandHandler>();
        services.AddScoped<RecordArPaymentCommandHandler>();
        services.AddScoped<GetArTransactionsQueryHandler>();

        // ApTransaction
        services.AddScoped<CreateApTransactionCommandHandler>();
        services.AddScoped<RecordApPaymentCommandHandler>();
        services.AddScoped<GetApTransactionsQueryHandler>();

        // CashOutTransaction
        services.AddScoped<CreateCashOutTransactionCommandHandler>();
        services.AddScoped<GetCashOutTransactionsQueryHandler>();

        // Finance
        services.AddScoped<GetFInvoicesQueryHandler>();

        // Validators (all assemblies scanned from this project)
        services.AddValidatorsFromAssemblyContaining<LoginCommandHandler>();

        return services;
    }
}
