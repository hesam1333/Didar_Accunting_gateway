namespace Gateway.Application.Dtos.Accounting.Requests;

public abstract record AccountingRequestBase
{
    public string ApiKey { get; init; } = string.Empty;
    public string LoginToken { get; init; } = string.Empty;
    public Guid BizDomainId { get; init; }
}
