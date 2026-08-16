using Gateway.Application.Dtos.Accounting.Requests;
using Gateway.Domain.Enums;

namespace Gateway.Application.Commands;

public sealed record BatchSaveItemsCommand(IReadOnlyList<ItemRequestDto> Items) : Command
{
    public override CommandType CommandType => CommandType.BatchSaveItems;
}
