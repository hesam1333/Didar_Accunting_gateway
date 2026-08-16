using Gateway.Domain.Enums;
using Gateway.Application.Dtos.Accounting.Requests;

namespace Gateway.Application.Commands;

public sealed record SaveInvoiceCommand(InvoiceRequestDto Invoice) : Command
{
    public override CommandType CommandType => CommandType.SaveInvoice;
}
