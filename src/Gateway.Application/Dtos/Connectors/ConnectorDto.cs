namespace Gateway.Application.Dtos.Connectors;

public record ConnectorDto(
    Guid Id,
    Guid BizDomainId,
    string ApiKey,
    string LoginToken,
    string AgentSecret,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiredAt,
    DateTimeOffset? LastAgentSeenAt);
