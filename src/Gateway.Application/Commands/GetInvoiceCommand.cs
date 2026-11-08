using Gateway.Domain.Enums;
namespace Gateway.Application.Commands;

public sealed record GetInvoiceCommand(int Number, int Type, int Status) : Command
{
    public override CommandType CommandType => CommandType.GetInvoice;
}
