using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Commands.Dealers.RegisterDealer;
using Cassandra.Application.Commands.Dealers.RenameDealer;
using Cassandra.Application.Commands.Dealers.SetDealerStatus;
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
using Cassandra.Application.Commands.TipeMotor.CreateTipeMotor;
using Cassandra.Application.Commands.TipeMotor.SetTipeMotorColors;
using Cassandra.Application.Commands.TipeMotor.SetTipeMotorStatus;
using Cassandra.Application.Commands.TipeMotor.UpdateTipeMotor;
using Cassandra.Application.Commands.Warna.CreateWarna;
using Cassandra.Application.Commands.Warna.SetWarnaStatus;
using Cassandra.Application.Commands.Warna.UpdateWarna;
using Cassandra.Application.Queries.Dealers;
using Cassandra.Application.Queries.GrupTipeMotor;
using Cassandra.Application.Queries.Jabatan;
using Cassandra.Application.Queries.Karyawan;
using Cassandra.Application.Queries.Kelengkapan;
using Cassandra.Application.Queries.Kios;
using Cassandra.Application.Queries.Mediator;
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

        // Validators (all assemblies scanned from this project)
        services.AddValidatorsFromAssemblyContaining<LoginCommandHandler>();

        return services;
    }
}
