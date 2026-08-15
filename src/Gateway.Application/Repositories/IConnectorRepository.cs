using Gateway.Domain.Entities;
using Gateway.Domain.Enums;

namespace Gateway.Application.Repositories;

public interface IConnectorRepository
{
    Task<Connector?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Connector?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken);

    Task<Connector?> GetByBizDomainIdAndProviderAsync(Guid bizDomainId, ProviderType provider, CancellationToken cancellationToken);

    Task<List<Connector>> GetByBizDomainIdAsync(Guid bizDomainId, CancellationToken cancellationToken);

    Task<(List<Connector> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task AddAsync(Connector connector, CancellationToken cancellationToken);

    Task UpdateAsync(Connector connector, CancellationToken cancellationToken);
}
