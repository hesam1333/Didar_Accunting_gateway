namespace Gateway.Application.Dtos.Accounting.Requests;

public sealed record BatchSaveItemsRequest : AccountingRequestBase
{
    public IReadOnlyList<ItemRequestDto> Items { get; init; } = new List<ItemRequestDto>();
}
