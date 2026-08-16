namespace Gateway.Application.Dtos.Accounting.Requests;

public sealed record SearchContactsRequest : AccountingRequestBase
{
    public QueryInfoDto QueryInfo { get; init; } = new();
}
