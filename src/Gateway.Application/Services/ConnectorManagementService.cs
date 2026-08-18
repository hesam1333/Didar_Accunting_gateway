using System.Security.Cryptography;
using System.Text;
using Gateway.Application.Dtos.Accounting.Requests;
using Gateway.Application.Dtos.Connectors;
using Gateway.Application.Exceptions;
using Gateway.Application.Repositories;
using Gateway.Application.Services.Abstractions;
using Gateway.Domain.Entities;

namespace Gateway.Application.Services;

public class ConnectorManagementService : IConnectorManagementService
{
    private readonly IConnectorRepository _connectorRepository;
    private readonly ICredentialEncryptionService _credentialEncryptionService;

    public ConnectorManagementService(
        IConnectorRepository connectorRepository,
        ICredentialEncryptionService credentialEncryptionService)
    {
        _connectorRepository = connectorRepository;
        _credentialEncryptionService = credentialEncryptionService;
    }

    public async Task<Guid> AuthenticateAsync(AccountingRequestBase request, CancellationToken cancellationToken)
    {
        var connector = await _connectorRepository.GetByApiKeyAsync(request.ApiKey, cancellationToken);

        if (connector is null)
        {
            throw new InvalidConnectorCredentialsException();
        }

        if (!connector.IsActive)
        {
            throw new InvalidConnectorCredentialsException("The connector is disabled.");
        }

        if (connector.BizDomainId != request.BizDomainId)
        {
            throw new InvalidConnectorCredentialsException();
        }

        var expectedLoginToken = _credentialEncryptionService.Decrypt(connector.LoginTokenEncrypted);

        if (!SecretsMatch(request.LoginToken, expectedLoginToken))
        {
            throw new InvalidConnectorCredentialsException();
        }

        return connector.Id;
    }

    public async Task<ConnectorDto> CreateAsync(CreateConnectorDto request, CancellationToken cancellationToken)
    {
        var existing = await _connectorRepository.GetByBizDomainIdAsync(request.BizDomainId, cancellationToken);

        if (existing is not null)
        {
            return ToDto(existing, DecryptLoginToken(existing), DecryptAgentSecret(existing));
        }

        var now = DateTimeOffset.UtcNow;
        var loginToken = GenerateSecret("lt_");
        var agentSecret = GenerateSecret("as_");

        var connector = new Connector
        {
            Id = Guid.NewGuid(),
            BizDomainId = request.BizDomainId,
            ApiKey = GenerateSecret("ak_"),
            LoginTokenEncrypted = _credentialEncryptionService.Encrypt(loginToken),
            AgentSecretEncrypted = _credentialEncryptionService.Encrypt(agentSecret),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _connectorRepository.AddAsync(connector, cancellationToken);

        return ToDto(connector, loginToken, agentSecret);
    }

    public async Task<ConnectorDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var connector = await GetRequiredConnectorAsync(id, cancellationToken);

        return ToDto(connector, DecryptLoginToken(connector), DecryptAgentSecret(connector));
    }

    public async Task<ConnectorListResultDto> ListAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var (connectors, totalCount) = await _connectorRepository.GetPagedAsync(page, pageSize, cancellationToken);

        var items = connectors
            .Select(connector => ToDto(connector, DecryptLoginToken(connector), DecryptAgentSecret(connector)))
            .ToList();

        return new ConnectorListResultDto(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyList<ConnectorDto>> ListByBizDomainIdAsync(Guid bizDomainId, CancellationToken cancellationToken)
    {
        var connector = await _connectorRepository.GetByBizDomainIdAsync(bizDomainId, cancellationToken);

        if (connector is null)
        {
            return Array.Empty<ConnectorDto>();
        }

        return new[] { ToDto(connector, DecryptLoginToken(connector), DecryptAgentSecret(connector)) };
    }

    public async Task<ConnectorDto> RotateLoginTokenAsync(Guid id, CancellationToken cancellationToken)
    {
        var connector = await GetRequiredConnectorAsync(id, cancellationToken);
        var loginToken = GenerateSecret("lt_");

        connector.LoginTokenEncrypted = _credentialEncryptionService.Encrypt(loginToken);
        connector.UpdatedAt = DateTimeOffset.UtcNow;

        await _connectorRepository.UpdateAsync(connector, cancellationToken);

        return ToDto(connector, loginToken, DecryptAgentSecret(connector));
    }

    public async Task<ConnectorDto> RotateAgentSecretAsync(Guid id, CancellationToken cancellationToken)
    {
        var connector = await GetRequiredConnectorAsync(id, cancellationToken);
        var agentSecret = GenerateSecret("as_");

        connector.AgentSecretEncrypted = _credentialEncryptionService.Encrypt(agentSecret);
        connector.UpdatedAt = DateTimeOffset.UtcNow;

        await _connectorRepository.UpdateAsync(connector, cancellationToken);

        return ToDto(connector, DecryptLoginToken(connector), agentSecret);
    }

    public async Task<ConnectorDto> EnableAsync(Guid id, CancellationToken cancellationToken)
    {
        var connector = await GetRequiredConnectorAsync(id, cancellationToken);

        connector.IsActive = true;
        connector.UpdatedAt = DateTimeOffset.UtcNow;

        await _connectorRepository.UpdateAsync(connector, cancellationToken);

        return ToDto(connector, DecryptLoginToken(connector), DecryptAgentSecret(connector));
    }

    public async Task<ConnectorDto> DisableAsync(Guid id, CancellationToken cancellationToken)
    {
        var connector = await GetRequiredConnectorAsync(id, cancellationToken);

        connector.IsActive = false;
        connector.UpdatedAt = DateTimeOffset.UtcNow;

        await _connectorRepository.UpdateAsync(connector, cancellationToken);

        return ToDto(connector, DecryptLoginToken(connector), DecryptAgentSecret(connector));
    }

    private async Task<Connector> GetRequiredConnectorAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _connectorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ConnectorNotFoundException(id);
    }

    private string DecryptLoginToken(Connector connector)
    {
        return _credentialEncryptionService.Decrypt(connector.LoginTokenEncrypted);
    }

    private string DecryptAgentSecret(Connector connector)
    {
        return _credentialEncryptionService.Decrypt(connector.AgentSecretEncrypted);
    }

    private static ConnectorDto ToDto(Connector connector, string loginToken, string agentSecret)
    {
        return new ConnectorDto(
            connector.Id,
            connector.BizDomainId,
            connector.ApiKey,
            loginToken,
            agentSecret,
            connector.IsActive,
            connector.CreatedAt,
            connector.LastAgentSeenAt);
    }

    private static string GenerateSecret(string prefix)
    {
        return prefix + Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
    }

    private static bool SecretsMatch(string actual, string expected)
    {
        var actualBytes = Encoding.UTF8.GetBytes(actual);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        return actualBytes.Length == expectedBytes.Length
            && CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
    }
}
