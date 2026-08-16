using Gateway.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gateway.Api.Swagger;

public class ConnectorManagementSecretOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requiresSecret = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .OfType<ServiceFilterAttribute>()
            .Any(attribute => attribute.ServiceType == typeof(ConnectorManagementSecretFilter));

        if (!requiresSecret)
        {
            return;
        }

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ConnectorManagementSecret"
                        }
                    },
                    new List<string>()
                }
            }
        };
    }
}
