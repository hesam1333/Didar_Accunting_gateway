using Gateway.Application.Commands;

namespace Gateway.Application.Services.Abstractions;

public interface IConnectorCommandDispatcher
{
    Task<CommandResult<TData>> DispatchAsync<TData>(Guid connectorId, Command command, CancellationToken cancellationToken);
}
