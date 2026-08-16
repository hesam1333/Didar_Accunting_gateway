namespace Gateway.Application.Dtos.Accounting.Requests;

public sealed record SaveInvoiceRequest : AccountingRequestBase
{
    public InvoiceRequestDto Invoice { get; init; } = new();
}
