using Gateway.Domain.Enums;
namespace Gateway.Application.Commands;

public sealed record GetInvoiceCommand(int Number, int Type) : Command
{
    public override CommandType CommandType => CommandType.GetInvoice;
}
