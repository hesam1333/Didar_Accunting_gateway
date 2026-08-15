using Gateway.Domain.Enums;
namespace Gateway.Application.Commands;

public sealed record GetContactCommand(string Code) : Command
{
    public override CommandType CommandType => CommandType.GetContact;
}
