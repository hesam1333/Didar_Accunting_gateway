using Gateway.Application.Dtos.Accounting.Requests;

namespace Gateway.Application.Dtos.Accounting.Responses;

public record InvoiceResponse
{
    public int Id { get; init; }

    public string Number { get; init; } = string.Empty;

    public DateTime Date { get; init; }

    public DateTime DueDate { get; init; }

    public string ContactCode { get; init; } = string.Empty;

    public ContactResponse Contact { get; init; } = new();

    public string ContactTitle { get; init; } = string.Empty;

    public decimal Sum { get; init; }

    public decimal Payable { get; init; }

    public decimal Paid { get; init; }

    public decimal Rest { get; init; }

    public decimal Freight { get; init; }

    public string FreightPersonCode { get; init; } = string.Empty;

    public string FreightPersonName { get; init; } = string.Empty;

    public string Note { get; init; } = string.Empty;

    public bool Sent { get; init; }

    public bool Returned { get; init; }

    public int InvoiceType { get; init; }

    public int Status { get; init; }

    public int WarehouseReceiptStatus { get; init; }

    public IReadOnlyList<InvoiceItemResponse> InvoiceItems { get; init; } = new List<InvoiceItemResponse>();

    public string Tag { get; init; } = string.Empty;

    public string Project { get; init; } = string.Empty;

    public string SalesmanCode { get; init; } = string.Empty;

    public double SalesmanPercent { get; init; }

    public IReadOnlyList<OtherCostDto> Others { get; init; } = new List<OtherCostDto>();

    public string Currency { get; init; } = string.Empty;

    public decimal CurrencyRate { get; init; }

    public string TaxId { get; init; } = string.Empty;
}
