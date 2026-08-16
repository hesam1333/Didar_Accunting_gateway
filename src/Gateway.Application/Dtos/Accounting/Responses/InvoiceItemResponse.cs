namespace Gateway.Application.Dtos.Accounting.Responses;

public record InvoiceItemResponse
{
    public int Id { get; init; }

    public int RowNumber { get; init; }

    public string Description { get; init; } = string.Empty;

    public string ItemCode { get; init; } = string.Empty;

    public ItemResponse Item { get; init; } = new();

    public decimal Quantity { get; init; }

    public string Unit { get; init; } = string.Empty;

    public decimal UnitPrice { get; init; }

    public decimal Sum { get; init; }

    public decimal Discount { get; init; }

    public decimal Tax { get; init; }

    public decimal TotalAmount { get; init; }

    public IReadOnlyList<string> SerialNumbers { get; init; } = new List<string>();
}
