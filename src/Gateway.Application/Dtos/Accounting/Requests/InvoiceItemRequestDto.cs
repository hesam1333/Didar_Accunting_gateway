namespace Gateway.Application.Dtos.Accounting.Requests;

public record InvoiceItemRequestDto
{
    public int? RowNumber { get; init; }

    public string Description { get; init; } = string.Empty;

    public string? ItemCode { get; init; }

    public string Unit { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal Discount { get; init; }

    public decimal Tax { get; init; }
}
