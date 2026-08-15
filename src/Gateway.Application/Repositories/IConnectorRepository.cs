using Gateway.Domain.Entities;

namespace Gateway.Application.Repositories;

public interface IConnectorRepository
{
    Task<Connector?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Connector?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken);

    Task<List<Connector>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Connector connector, CancellationToken cancellationToken);

    Task UpdateAsync(Connector connector, CancellationToken cancellationToken);
}
