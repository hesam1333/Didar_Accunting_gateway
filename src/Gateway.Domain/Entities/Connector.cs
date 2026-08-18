namespace Gateway.Domain.Entities;

public class Connector
{
    public Guid Id { get; set; }

    public Guid BizDomainId { get; set; } 

    public string ApiKey { get; set; } = string.Empty;

    public string LoginTokenEncrypted { get; set; } = string.Empty;

    public string AgentSecretEncrypted { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public DateTimeOffset? LastAgentConnectedAt { get; set; }

    public DateTimeOffset? LastAgentSeenAt { get; set; }
}
