using Gateway.Application.Services.Abstractions;

namespace Gateway.Infrastructure.SignalR;

public class AgentIdentity : IAgentIdentity
{
    public Guid ConnectorId { get; init; }
    public Guid BizDomainId { get; init; }
}
