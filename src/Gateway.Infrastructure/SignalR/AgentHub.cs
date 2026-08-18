using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Gateway.Infrastructure.SignalR;

[Authorize]
public class AgentHub : Hub
{
    private readonly ConnectorConnectionRegistry _connectionRegistry;
    private readonly PendingCommandRegistry _pendingCommandRegistry;
    private readonly ILogger<AgentHub> _logger;

    public AgentHub(
        ConnectorConnectionRegistry connectionRegistry,
        PendingCommandRegistry pendingCommandRegistry,
        ILogger<AgentHub> logger)
    {
        _connectionRegistry = connectionRegistry;
        _pendingCommandRegistry = pendingCommandRegistry;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var connectorId = GetConnectorId();

        if (connectorId is null)
        {
            _logger.LogWarning("Agent connection rejected: ConnectorId claim is missing or invalid.");
            Context.Abort();
            return;
        }

        _connectionRegistry.Register(connectorId.Value, Context.ConnectionId);

        _logger.LogInformation("Agent connected. ConnectorId={ConnectorId}, ConnectionId={ConnectionId}",
            connectorId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectorId = GetConnectorId();

        if (connectorId is not null)
        {
            _connectionRegistry.Unregister(connectorId.Value, Context.ConnectionId);
            FailPendingCommands(connectorId.Value);

            _logger.LogInformation("Agent disconnected. ConnectorId={ConnectorId}, ConnectionId={ConnectionId}",
                connectorId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public Task SubmitCommandResult(CommandResultEnvelope result)
    {
        var connectorId = GetConnectorId();

        if (connectorId is not null)
        {
            _pendingCommandRegistry.TryComplete(connectorId.Value, result.CommandId, result);
        }

        return Task.CompletedTask;
    }

    private Guid? GetConnectorId()
    {
        var value = Context.User?.FindFirstValue("ConnectorId");

        if (Guid.TryParse(value, out var connectorId))
        {
            return connectorId;
        }

        return null;
    }

    private void FailPendingCommands(Guid connectorId)
    {
        var allPending = _pendingCommandRegistry.GetAll();

        foreach (var pending in allPending)
        {
            if (pending.ConnectorId == connectorId)
            {
                _pendingCommandRegistry.TryComplete(connectorId, pending.CommandId, new CommandResultEnvelope(
                    pending.CommandId,
                    Success: false,
                    ErrorCode: "AgentDisconnected",
                    ErrorMessage: "The agent disconnected before the command could complete.",
                    Data: null));
            }
        }
    }
}
