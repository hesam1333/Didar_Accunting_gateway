using Gateway.Application.Dtos.Accounting;
using Gateway.Domain.Enums;

namespace Gateway.Application.Commands;

public sealed record BatchSaveItemsCommand(IReadOnlyList<ItemDto> Items) : Command
{
    public override CommandType CommandType => CommandType.BatchSaveItems;
}
