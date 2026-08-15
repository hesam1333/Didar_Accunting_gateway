using Gateway.Domain.Enums;

namespace Gateway.Application.Commands;

public sealed record GetBusinessInfoCommand : Command
{
    public override CommandType CommandType => CommandType.GetBusinessInfo;
}
