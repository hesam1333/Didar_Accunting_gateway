using Gateway.Domain.Enums;
using Gateway.Application.Dtos.Accounting;

namespace Gateway.Application.Commands;

public sealed record SaveInvoiceCommand(InvoiceDto Invoice) : Command
{
    public override CommandType CommandType => CommandType.SaveInvoice;
}
