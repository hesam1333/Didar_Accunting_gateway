using Gateway.Domain.Enums;

namespace Gateway.Application.Commands;

public sealed record DeleteInvoiceCommand(int Number, int Type) : Command
{
    public override CommandType CommandType => CommandType.DeleteInvoice;
}
