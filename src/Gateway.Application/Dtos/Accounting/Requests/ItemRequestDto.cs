namespace Gateway.Application.Dtos.Accounting.Requests;

public record ItemRequestDto
{
    public string? Code { get; init; }

    public string Name { get; init; } = string.Empty;

    public int ItemType { get; init; }

    public string Unit { get; init; } = string.Empty;

    public decimal SellPrice { get; init; }

    public string SalesTitle { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ProductCode { get; init; } = string.Empty;

    public bool Active { get; init; }

    public string NodeFamily { get; init; } = string.Empty;
}
