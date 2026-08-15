using Gateway.Application.Repositories;
using Gateway.Application.Services.Abstractions;
using Gateway.Infrastructure.Persistence;
using Gateway.Infrastructure.Repositories;
using Gateway.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountingGatewayDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("GatewayDatabase")));

        services.AddScoped<IConnectorRepository, ConnectorRepository>();
        services.AddScoped<ICredentialEncryptionService, CredentialEncryptionService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
