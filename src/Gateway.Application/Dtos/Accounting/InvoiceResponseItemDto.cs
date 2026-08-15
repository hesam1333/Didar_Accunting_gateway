namespace Gateway.Application.Dtos.Accounting;

public record InvoiceResponseItemDto(
    int Id,
    int RowNumber,
    string Description,
    string ItemCode,
    ItemDto Item,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    decimal Sum,
    decimal Discount,
    decimal Tax,
    decimal TotalAmount,
    IReadOnlyList<string> SerialNumbers);
