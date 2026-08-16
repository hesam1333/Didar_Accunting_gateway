namespace Gateway.Application.Dtos.Accounting.Requests;

public abstract record AccountingRequestBase
{
    public string ApiKey { get; init; } = string.Empty;

    public string LoginToken { get; init; } = string.Empty;

    public int ProviderType { get; init; } = 3;

    public Guid UserId { get; init; }

    public Guid BizDomainId { get; init; }
}
