using Gateway.Api;
using Gateway.Application;
using Gateway.Application.Configuration;
using Gateway.Infrastructure;
using Gateway.Infrastructure.Persistence;
using Gateway.Infrastructure.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthorization();

builder.Services.AddSwagger();

builder.Services.AddSingleton(new ConnectorExpirationOptions
{
    TrialDays = builder.Configuration.GetValue<int?>("TrialDays") ?? 14
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AccountingGatewayDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

if (app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AgentHub>("/hubs/accounting-agent");

app.Run();
