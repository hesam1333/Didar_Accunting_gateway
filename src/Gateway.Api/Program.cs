using Gateway.Api.Filters;
using Gateway.Api.Swagger;
using Gateway.Application;
using Gateway.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<ConnectorManagementSecretFilter>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
