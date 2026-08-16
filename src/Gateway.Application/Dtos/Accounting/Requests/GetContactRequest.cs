namespace Gateway.Application.Dtos.Accounting.Requests;

public sealed record GetContactRequest : AccountingRequestBase
{
    public string Code { get; init; } = string.Empty;
}
