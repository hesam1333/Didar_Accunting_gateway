using System.Text.Json;
using Gateway.Application.Commands;
using Gateway.Application.Exceptions;
using Gateway.Application.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace Gateway.Infrastructure.SignalR;

public class SignalRConnectorCommandDispatcher : IConnectorCommandDispatcher
{
    private readonly IHubContext<AgentHub> _hubContext;
    private readonly ConnectorConnectionRegistry _connectionRegistry;
    private readonly PendingCommandRegistry _pendingCommandRegistry;
    private readonly TimeSpan _commandTimeout;

    public SignalRConnectorCommandDispatcher(
        IHubContext<AgentHub> hubContext,
        ConnectorConnectionRegistry connectionRegistry,
        PendingCommandRegistry pendingCommandRegistry,
        IConfiguration configuration)
    {
        _hubContext = hubContext;
        _connectionRegistry = connectionRegistry;
        _pendingCommandRegistry = pendingCommandRegistry;

        var timeoutSeconds = int.TryParse(configuration["SignalR:CommandTimeoutSeconds"], out var seconds)
            ? seconds
            : 30;

        _commandTimeout = TimeSpan.FromSeconds(timeoutSeconds);
    }

    public async Task<CommandResult<TData>> DispatchAsync<TData>(
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

            using var timeoutCts = new CancellationTokenSource(_commandTimeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                await _hubContext.Clients.Client(connectionId)
                    .SendCoreAsync("ExecuteCommand", new object[] { envelope }, linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                throw CommandDispatchException.Timeout(connectorId, _commandTimeout);
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
                throw CommandDispatchException.Timeout(connectorId, _commandTimeout);
            }
            catch (OperationCanceledException)
            {
                throw;
            }

            return new CommandResult<TData>
            {
                CommandId = result.CommandId,
                Success = result.Success,
                ErrorCode = result.ErrorCode,
                ErrorMessage = result.ErrorMessage,
                Data = DeserializeData<TData>(result.Data)
            };
        }
        catch
        {
            _pendingCommandRegistry.TryCancel(command.CommandId);
            throw;
        }
    }

    private static TData? DeserializeData<TData>(object? data)
    {
        if (data is null)
        {
            return default;
        }

        if (data is TData typed)
        {
            return typed;
        }

        if (data is JsonElement element)
        {
            return element.Deserialize<TData>();
        }

        return (TData)data;
    }
}
