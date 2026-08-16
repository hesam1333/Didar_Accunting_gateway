namespace Gateway.Application.Dtos.Accounting.Responses;

public record BusinessInfoResponse
{
    public string Name { get; init; } = string.Empty;

    public string LegalName { get; init; } = string.Empty;

    public string Calendar { get; init; } = string.Empty;

    public string Currency { get; init; } = string.Empty;

    public bool SupportMultiCurrency { get; init; }

    public string OtherCurrencies { get; init; } = string.Empty;

    public int Subscription { get; init; }

    public int Credit { get; init; }

    public DateTime ExpireDate { get; init; }
}
