namespace Gateway.Domain;

public sealed class Connector
{
    public Guid Id { get; private set; }

    public Guid BizDomainId { get; private set; }

    public string Provider { get; private set; }

    public string ApiKey { get; private set; }

    public string LoginTokenEncrypted { get; private set; }

    public string AgentSecretEncrypted { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? LastAgentConnectedAt { get; private set; }

    public DateTimeOffset? LastAgentSeenAt { get; private set; }

    private Connector()
    {
        Provider = string.Empty;
        ApiKey = string.Empty;
        LoginTokenEncrypted = string.Empty;
        AgentSecretEncrypted = string.Empty;
    }

    public Connector(
        Guid id,
        Guid bizDomainId,
        string provider,
        string apiKey,
        string loginTokenEncrypted,
        string agentSecretEncrypted)
    {
        Id = id;
        BizDomainId = bizDomainId;
        Provider = provider;
        ApiKey = apiKey;
        LoginTokenEncrypted = loginTokenEncrypted;
        AgentSecretEncrypted = agentSecretEncrypted;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Enable()
    {
        IsActive = true;
        Touch();
    }

    public void Disable()
    {
        IsActive = false;
        Touch();
    }

    public void RotateLoginToken(string newLoginTokenEncrypted)
    {
        LoginTokenEncrypted = newLoginTokenEncrypted;
        Touch();
    }

    public void RotateAgentSecret(string newAgentSecretEncrypted)
    {
        AgentSecretEncrypted = newAgentSecretEncrypted;
        Touch();
    }

    public void MarkAgentConnected(DateTimeOffset utcNow)
    {
        LastAgentConnectedAt = utcNow;
        LastAgentSeenAt = utcNow;
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
