using Gateway.Api.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Gateway.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("ConnectorManagementSecret", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "auth-con-manager",
                In = ParameterLocation.Header,
                Description = "Connector management secret required for connector management endpoints."
            });

            options.OperationFilter<ConnectorManagementSecretOperationFilter>();
        });

        return services;
    }
}
