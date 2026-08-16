namespace Gateway.Application.Dtos.Accounting.Requests;

public sealed record SearchItemsRequest : AccountingRequestBase
{
    public QueryInfoDto QueryInfo { get; init; } = new();
}
