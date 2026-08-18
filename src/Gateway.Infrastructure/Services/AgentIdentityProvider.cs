using System.Security.Claims;
using Gateway.Application.Services.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Gateway.Infrastructure.Services;

public class AgentIdentityProvider : IAgentIdentityProvider
{
    private const string ConnectorIdClaimType = "ConnectorId";
    private const string BizDomainIdClaimType = "BizDomainId";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public AgentIdentityProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IAgentIdentity GetCurrent()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            throw new InvalidOperationException("No authenticated agent identity is available.");
        }

        var connectorIdValue = user.FindFirstValue(ConnectorIdClaimType);
        if (string.IsNullOrEmpty(connectorIdValue) || !Guid.TryParse(connectorIdValue, out var connectorId))
        {
            throw new InvalidOperationException("The authenticated agent identity does not contain a valid ConnectorId claim.");
        }

        var bizDomainIdValue = user.FindFirstValue(BizDomainIdClaimType);
        if (string.IsNullOrEmpty(bizDomainIdValue) || !Guid.TryParse(bizDomainIdValue, out var bizDomainId))
        {
            throw new InvalidOperationException("The authenticated agent identity does not contain a valid BizDomainId claim.");
        }

        return new SignalR.AgentIdentity
        {
            ConnectorId = connectorId,
            BizDomainId = bizDomainId
        };
    }
}
