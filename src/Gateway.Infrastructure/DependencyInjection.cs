using System.Text;
using Gateway.Application.Repositories;
using Gateway.Application.Services.Abstractions;
using Gateway.Infrastructure.Authentication;
using Gateway.Infrastructure.Persistence;
using Gateway.Infrastructure.Repositories;
using Gateway.Infrastructure.Services;
using Gateway.Infrastructure.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddJwtAuthentication(configuration);

        services.AddSignalR();

        services.AddHttpContextAccessor();

        services.AddDbContext<AccountingGatewayDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("GatewayDatabase")));

        services.AddScoped<ConnectorManagementSecretFilter>();


        services.AddScoped<IConnectorRepository, ConnectorRepository>();
        services.AddScoped<ICredentialEncryptionService, CredentialEncryptionService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAgentIdentityProvider, AgentIdentityProvider>();

        services.AddSingleton<ConnectorConnectionRegistry>();
        services.AddSingleton<PendingCommandRegistry>();
        services.AddSingleton<IConnectorCommandDispatcher, SignalRConnectorCommandDispatcher>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSection = configuration.GetSection("Jwt");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSection["Key"]!))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/accounting-agent"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}
