using Gateway.Application.Dtos.Agents;
using Gateway.Application.Exceptions;
using Gateway.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Api.Controllers;

[ApiController]
[Route("api/agents")]
public class AgentAuthenticationController : ControllerBase
{
    private readonly IAgentAuthenticationService _agentAuthenticationService;

    public AgentAuthenticationController(IAgentAuthenticationService agentAuthenticationService)
    {
        _agentAuthenticationService = agentAuthenticationService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AgentLoginResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync(AgentLoginRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _agentAuthenticationService.LoginAsync(request, cancellationToken);

            return Ok(result);
        }
        catch (InvalidAgentCredentialsException)
        {
            return Unauthorized();
        }
    }
}
