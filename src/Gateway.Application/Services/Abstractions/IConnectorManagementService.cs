using Gateway.Application.Dtos.Accounting.Requests;
using Gateway.Application.Dtos.Connectors;

namespace Gateway.Application.Services.Abstractions;

public interface IConnectorManagementService
{
    Task<Guid> AuthenticateAsync(AccountingRequestBase request, CancellationToken cancellationToken);


    Task<ConnectorDto> CreateAsync(CreateConnectorDto request, CancellationToken cancellationToken);

    Task<ConnectorDto> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<ConnectorListResultDto> ListAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<IReadOnlyList<ConnectorDto>> ListByBizDomainIdAsync(Guid bizDomainId, CancellationToken cancellationToken);

    Task<ConnectorDto> RotateLoginTokenAsync(Guid id, CancellationToken cancellationToken);

    Task<ConnectorDto> RotateAgentSecretAsync(Guid id, CancellationToken cancellationToken);

    Task<ConnectorDto> EnableAsync(Guid id, CancellationToken cancellationToken);

    Task<ConnectorDto> DisableAsync(Guid id, CancellationToken cancellationToken);

    Task<ConnectorDto> UpdateExpirationAsync(Guid id, DateTimeOffset? expiredAt, CancellationToken cancellationToken);
}
