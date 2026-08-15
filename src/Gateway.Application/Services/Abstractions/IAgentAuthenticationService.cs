using Gateway.Application.Dtos.Agents;

namespace Gateway.Application.Services.Abstractions;

public interface IAgentAuthenticationService
{
    Task<AgentLoginResultDto> LoginAsync(AgentLoginRequestDto request, CancellationToken cancellationToken);
}
