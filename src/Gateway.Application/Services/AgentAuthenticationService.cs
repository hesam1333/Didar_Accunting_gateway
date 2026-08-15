using System.Security.Cryptography;
using System.Text;
using Gateway.Application.Dtos.Agents;
using Gateway.Application.Exceptions;
using Gateway.Application.Repositories;
using Gateway.Application.Services.Abstractions;

namespace Gateway.Application.Services;

public class AgentAuthenticationService : IAgentAuthenticationService
{
    private readonly IConnectorRepository _connectorRepository;
    private readonly ICredentialEncryptionService _credentialEncryptionService;
    private readonly IJwtTokenService _jwtTokenService;

    public AgentAuthenticationService(
        IConnectorRepository connectorRepository,
        ICredentialEncryptionService credentialEncryptionService,
        IJwtTokenService jwtTokenService)
    {
        _connectorRepository = connectorRepository;
        _credentialEncryptionService = credentialEncryptionService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AgentLoginResultDto> LoginAsync(AgentLoginRequestDto request, CancellationToken cancellationToken)
    {
        var connector = await _connectorRepository.GetByIdAsync(request.ConnectorId, cancellationToken);

        if (connector is null || !connector.IsActive)
        {
            throw new InvalidAgentCredentialsException();
        }

        var expectedAgentSecret = _credentialEncryptionService.Decrypt(connector.AgentSecretEncrypted);

        if (!SecretsMatch(request.AgentSecret, expectedAgentSecret))
        {
            throw new InvalidAgentCredentialsException();
        }

        var token = _jwtTokenService.GenerateAgentToken(connector);

        return new AgentLoginResultDto(token);
    }

    private static bool SecretsMatch(string actual, string expected)
    {
        var actualBytes = Encoding.UTF8.GetBytes(actual);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        return actualBytes.Length == expectedBytes.Length
            && CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
    }
}
