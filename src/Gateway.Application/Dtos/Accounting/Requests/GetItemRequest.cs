namespace Gateway.Application.Dtos.Accounting.Requests;

public sealed record GetItemRequest : AccountingRequestBase
{
    public string Code { get; init; } = string.Empty;
}
