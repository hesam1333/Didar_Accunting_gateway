using Gateway.Application.Services;
using Gateway.Application.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IConnectorManagementService, ConnectorManagementService>();
        services.AddScoped<IAgentAuthenticationService, AgentAuthenticationService>();

        return services;
    }
}
