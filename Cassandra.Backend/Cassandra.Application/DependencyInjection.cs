using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Commands.Dealers.RegisterDealer;
using Cassandra.Application.Commands.Dealers.RenameDealer;
using Cassandra.Application.Commands.Dealers.SetDealerStatus;
using Cassandra.Application.Commands.Jabatan.CreateJabatan;
using Cassandra.Application.Commands.Jabatan.SetJabatanStatus;
using Cassandra.Application.Commands.Jabatan.UpdateJabatan;
using Cassandra.Application.Queries.Dealers;
using Cassandra.Application.Queries.Jabatan;
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

        // Validators (all assemblies scanned from this project)
        services.AddValidatorsFromAssemblyContaining<LoginCommandHandler>();

        return services;
    }
}
