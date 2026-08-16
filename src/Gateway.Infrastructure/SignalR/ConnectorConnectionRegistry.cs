using System.Collections.Concurrent;

namespace Gateway.Infrastructure.SignalR;

public class ConnectorConnectionRegistry
{
    private readonly ConcurrentDictionary<Guid, string> _connections = new();

    public void Register(Guid connectorId, string connectionId)
    {
        _connections[connectorId] = connectionId;
    }

    public void Unregister(Guid connectorId)
    {
        _connections.TryRemove(connectorId, out _);
    }

    public bool TryGetConnectionId(Guid connectorId, out string? connectionId)
    {
        return _connections.TryGetValue(connectorId, out connectionId);
    }

    public bool IsConnected(Guid connectorId)
    {
        return _connections.ContainsKey(connectorId);
    }
}
