using Gateway.Domain.Enums;
using Gateway.Application.Dtos.Accounting.Requests;

namespace Gateway.Application.Commands;

public sealed record SearchItemsCommand(QueryInfoDto QueryInfo) : Command
{
    public override CommandType CommandType => CommandType.SearchItems;
}
