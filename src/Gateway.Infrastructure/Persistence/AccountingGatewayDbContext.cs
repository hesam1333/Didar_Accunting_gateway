using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Persistence;

public class AccountingGatewayDbContext : DbContext
{
    public AccountingGatewayDbContext(DbContextOptions<AccountingGatewayDbContext> options)
        : base(options)
    {
    }

    public DbSet<Connector> Connectors => Set<Connector>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountingGatewayDbContext).Assembly);
    }
}
