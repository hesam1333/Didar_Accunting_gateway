using Gateway.Application.Commands;

namespace Gateway.Application.Services.Abstractions;

public interface IConnectorCommandDispatcher
{
    Task<CommandResult<object>> DispatchAsync(Guid connectorId, Command command, CancellationToken cancellationToken);
}
