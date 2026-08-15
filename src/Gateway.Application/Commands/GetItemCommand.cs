using Gateway.Domain.Enums;
namespace Gateway.Application.Commands;

public sealed record GetItemCommand(string Code) : Command
{
    public override CommandType CommandType => CommandType.GetItem;
}
