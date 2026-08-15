using Gateway.Application.Repositories;
using Gateway.Domain.Entities;
using Gateway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class ConnectorRepository : IConnectorRepository
{
    private readonly AccountingGatewayDbContext _dbContext;

    public ConnectorRepository(AccountingGatewayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Connector?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Connectors
            .FirstOrDefaultAsync(connector => connector.Id == id, cancellationToken);
    }

    public async Task<Connector?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken)
    {
        return await _dbContext.Connectors
            .FirstOrDefaultAsync(connector => connector.ApiKey == apiKey, cancellationToken);
    }

    public async Task<List<Connector>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Connectors
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Connector connector, CancellationToken cancellationToken)
    {
        await _dbContext.Connectors.AddAsync(connector, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Connector connector, CancellationToken cancellationToken)
    {
        _dbContext.Connectors.Update(connector);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
