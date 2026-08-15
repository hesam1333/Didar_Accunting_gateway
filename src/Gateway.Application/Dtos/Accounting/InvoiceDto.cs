namespace Gateway.Application.Dtos.Accounting;

public record InvoiceDto(
    string? Number,
    string? Reference,
    DateTime Date,
    DateTime DueDate,
    string ContactCode,
    string ContactTitle,
    string? Note,
    int InvoiceType,
    int Status,
    string? Tag,
    bool? Sent,
    decimal? Freight,
    int WarehouseReceiptStatus,
    string Project,
    string SalesmanCode,
    double? SalesmanPercent,
    IReadOnlyList<InvoiceItemDto> InvoiceItems,
    IReadOnlyList<OtherCostDto> Others,
    string Currency,
    decimal CurrencyRate);
