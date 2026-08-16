using Gateway.Domain.Enums;
using Gateway.Application.Dtos.Accounting.Requests;

namespace Gateway.Application.Commands;

public sealed record SaveContactCommand(ContactRequestDto? Contact) : Command
{
    public override CommandType CommandType => CommandType.SaveContact;
}
