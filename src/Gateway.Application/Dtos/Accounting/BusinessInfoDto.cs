namespace Gateway.Application.Dtos.Accounting;

public record BusinessInfoDto(
    string Name,
    string LegalName,
    string Calendar,
    string Currency,
    bool SupportMultiCurrency,
    string OtherCurrencies,
    int Subscription,
    int Credit,
    DateTime ExpireDate);
