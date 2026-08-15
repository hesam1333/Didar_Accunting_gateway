using Gateway.Domain.Enums;
using Gateway.Application.Dtos.Accounting;

namespace Gateway.Application.Commands;

public sealed record SearchContactsCommand(QueryInfoDto QueryInfo) : Command
{
    public override CommandType CommandType => CommandType.SearchContacts;
}
