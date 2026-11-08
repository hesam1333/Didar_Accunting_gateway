namespace Gateway.Application.Dtos.Accounting.Requests;

public record InvoiceRequestDto
{
    public string? Number { get; init; }

    public string? Reference { get; init; }

    public DateTime Date { get; init; }

    public DateTime DueDate { get; init; }

    public string? ContactCode { get; init; } = string.Empty;

    public string? ContactTitle { get; init; } = string.Empty;

    public string? Note { get; init; }

    public int InvoiceType { get; init; }

    public int Status { get; init; }

    public string? Tag { get; init; }

    public bool? Sent { get; init; }

    public decimal? Freight { get; init; }

    public int WarehouseReceiptStatus { get; init; }

    public string? Project { get; init; } = string.Empty;

    public string? SalesmanCode { get; init; } = string.Empty;

    public double? SalesmanPercent { get; init; }

    public IReadOnlyList<InvoiceItemRequestDto> InvoiceItems { get; init; } = new List<InvoiceItemRequestDto>();

    public IReadOnlyList<OtherCostDto> Others { get; init; } = new List<OtherCostDto>();

    public string? Currency { get; init; } = string.Empty;

    public decimal CurrencyRate { get; init; }
}
