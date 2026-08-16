namespace Gateway.Application.Dtos.Accounting.Requests;

public record OtherCostDto
{
    public string Title { get; init; } = string.Empty;

    public decimal Amount { get; init; }

    public bool Add { get; init; }
}
