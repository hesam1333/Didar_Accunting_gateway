using Gateway.Application.Commands;
using Gateway.Application.Exceptions;
using Gateway.Application.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Gateway.Infrastructure.SignalR;

public class SignalRConnectorCommandDispatcher : IConnectorCommandDispatcher
{
    private readonly IHubContext<AgentHub> _hubContext;
    private readonly ConnectorConnectionRegistry _connectionRegistry;
    private readonly PendingCommandRegistry _pendingCommandRegistry;

    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);

    public SignalRConnectorCommandDispatcher(
        IHubContext<AgentHub> hubContext,
        ConnectorConnectionRegistry connectionRegistry,
        PendingCommandRegistry pendingCommandRegistry)
    {
        _hubContext = hubContext;
        _connectionRegistry = connectionRegistry;
        _pendingCommandRegistry = pendingCommandRegistry;
    }

    public async Task<CommandResult<object>> DispatchAsync(
        Guid connectorId,
        Command command,
        CancellationToken cancellationToken)
    {
        if (!_connectionRegistry.TryGetConnectionId(connectorId, out var connectionId) || connectionId is null)
        {
            throw CommandDispatchException.AgentNotConnected(connectorId);
        }

        var pendingCommand = new PendingCommand(command.CommandId, connectorId);
        _pendingCommandRegistry.Add(pendingCommand);

        try
        {
            var envelope = new ConnectorCommandEnvelope(
                command.CommandId,
                command.CommandType,
                command);

            using var timeoutCts = new CancellationTokenSource(CommandTimeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                await _hubContext.Clients.Client(connectionId)
                    .SendCoreAsync("ExecuteCommand", new object[] { envelope }, linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                throw CommandDispatchException.Timeout(connectorId, CommandTimeout);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw CommandDispatchException.TransportFailure(connectorId, ex);
            }

            CommandResultEnvelope result;
            try
            {
                result = await pendingCommand.Completion.Task.WaitAsync(linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                throw CommandDispatchException.Timeout(connectorId, CommandTimeout);
            }
            catch (OperationCanceledException)
            {
                throw;
            }

            return new CommandResult<object>
            {
                CommandId = result.CommandId,
                Success = result.Success,
                ErrorCode = result.ErrorCode,
                ErrorMessage = result.ErrorMessage,
                Data = result.Data
            };
        }
        catch
        {
            _pendingCommandRegistry.TryCancel(command.CommandId);
            throw;
        }
    }
}
