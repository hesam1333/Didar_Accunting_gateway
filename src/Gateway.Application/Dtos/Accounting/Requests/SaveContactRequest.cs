namespace Gateway.Application.Dtos.Accounting.Requests;

public sealed record SaveContactRequest : AccountingRequestBase
{
    public ContactRequestDto? Contact { get; init; }
}
