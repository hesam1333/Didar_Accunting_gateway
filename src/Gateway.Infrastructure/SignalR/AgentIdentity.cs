using Gateway.Application.Services.Abstractions;
using Gateway.Domain.Enums;

namespace Gateway.Infrastructure.SignalR;

public class AgentIdentity : IAgentIdentity
{
    public Guid ConnectorId { get; init; }
    public Guid BizDomainId { get; init; }
    public ProviderType Provider { get; init; }
}
