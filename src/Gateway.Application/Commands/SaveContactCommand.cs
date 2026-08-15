using Gateway.Domain.Enums;
using Gateway.Application.Dtos.Accounting;

namespace Gateway.Application.Commands;

public sealed record SaveContactCommand(ContactDto Contact) : Command
{
    public override CommandType CommandType => CommandType.SaveContact;
}
