namespace Gateway.Application.Dtos.Accounting.Requests;

public sealed record GetInvoiceRequest : AccountingRequestBase
{
    public int Number { get; init; }

    public int Type { get; init; }
}
