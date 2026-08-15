using Gateway.Application.Dtos.Connectors;

namespace Gateway.Application.Services.Abstractions;

public interface IConnectorManagementService
{
    Task<ConnectorDto> CreateAsync(CreateConnectorDto request, CancellationToken cancellationToken);

    Task<ConnectorDto> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ConnectorDto>> ListAsync(CancellationToken cancellationToken);

    Task<ConnectorDto> RotateLoginTokenAsync(Guid id, CancellationToken cancellationToken);

    Task<ConnectorDto> RotateAgentSecretAsync(Guid id, CancellationToken cancellationToken);

    Task<ConnectorDto> EnableAsync(Guid id, CancellationToken cancellationToken);

    Task<ConnectorDto> DisableAsync(Guid id, CancellationToken cancellationToken);
}
