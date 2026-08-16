namespace Gateway.Application.Dtos.Accounting.Responses;

public record ItemResponse
{
    public int Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public int ItemType { get; init; }

    public string Unit { get; init; } = string.Empty;

    public string SubUnit { get; init; } = string.Empty;

    public double ConversionFactor { get; init; }

    public string Barcode { get; init; } = string.Empty;

    public double Stock { get; init; }

    public decimal BuyPrice { get; init; }

    public decimal SellPrice { get; init; }

    public string PurchasesTitle { get; init; } = string.Empty;

    public string SalesTitle { get; init; } = string.Empty;

    public int NodeId { get; init; }

    public string NodeName { get; init; } = string.Empty;

    public string NodeFamily { get; init; } = string.Empty;

    public string Tag { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ProductCode { get; init; } = string.Empty;

    public bool Active { get; init; }

    public string TaxId { get; init; } = string.Empty;

    public string TaxUnit { get; init; } = string.Empty;

    public object? PriceList { get; init; }
}
