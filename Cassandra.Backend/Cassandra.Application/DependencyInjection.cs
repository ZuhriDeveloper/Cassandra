using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Commands.Dealers.RegisterDealer;
using Cassandra.Application.Commands.Dealers.RenameDealer;
using Cassandra.Application.Commands.Dealers.SetDealerStatus;
using Cassandra.Application.Queries.Dealers;
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

        // Validators (all assemblies scanned from this project)
        services.AddValidatorsFromAssemblyContaining<LoginCommandHandler>();

        return services;
    }
}
