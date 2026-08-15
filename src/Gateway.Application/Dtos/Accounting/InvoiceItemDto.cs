namespace Gateway.Application.Dtos.Accounting;

public record InvoiceItemDto(
    int? RowNumber,
    string Description,
    string? ItemCode,
    string Unit,
    decimal Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal Tax);
