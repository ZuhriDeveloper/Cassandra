using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Commands.Dealers.RegisterDealer;
using Cassandra.Application.Commands.Dealers.RenameDealer;
using Cassandra.Application.Commands.Dealers.SetDealerStatus;
using Cassandra.Application.Commands.Jabatan.CreateJabatan;
using Cassandra.Application.Commands.Jabatan.SetJabatanStatus;
using Cassandra.Application.Commands.Jabatan.UpdateJabatan;
using Cassandra.Application.Commands.Karyawan.CreateKaryawan;
using Cassandra.Application.Commands.Karyawan.RecordKaryawanResign;
using Cassandra.Application.Commands.Karyawan.SetKaryawanLimit;
using Cassandra.Application.Commands.Karyawan.SetKaryawanStatus;
using Cassandra.Application.Commands.Karyawan.UpdateKaryawan;
using Cassandra.Application.Commands.Kios.CreateKios;
using Cassandra.Application.Commands.Kios.SetKiosLimit;
using Cassandra.Application.Commands.Kios.SetKiosStatus;
using Cassandra.Application.Commands.Kios.UpdateKios;
using Cassandra.Application.Queries.Dealers;
using Cassandra.Application.Queries.Jabatan;
using Cassandra.Application.Queries.Karyawan;
using Cassandra.Application.Queries.Kios;
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

        // Validators (all assemblies scanned from this project)
        services.AddValidatorsFromAssemblyContaining<LoginCommandHandler>();

        return services;
    }
}
