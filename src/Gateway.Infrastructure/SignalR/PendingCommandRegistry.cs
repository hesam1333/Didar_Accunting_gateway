using System.Collections.Concurrent;

namespace Gateway.Infrastructure.SignalR;

public class PendingCommandRegistry
{
    private readonly ConcurrentDictionary<Guid, PendingCommand> _pending = new();

    public void Add(PendingCommand pendingCommand)
    {
        if (!_pending.TryAdd(pendingCommand.CommandId, pendingCommand))
        {
            throw new InvalidOperationException($"A pending command with ID '{pendingCommand.CommandId}' already exists.");
        }
    }

    public bool TryComplete(Guid connectorId, Guid commandId, CommandResultEnvelope result)
    {
        if (!_pending.TryGetValue(commandId, out var pendingCommand))
        {
            return false;
        }

        if (pendingCommand.ConnectorId != connectorId)
        {
            return false;
        }

        if (!_pending.TryRemove(commandId, out pendingCommand))
        {
            return false;
        }

        pendingCommand.Completion.TrySetResult(result);
        return true;
    }

    public bool TryCancel(Guid commandId)
    {
        if (!_pending.TryRemove(commandId, out var pendingCommand))
        {
            return false;
        }

        pendingCommand.Completion.TrySetCanceled();
        return true;
    }

    public bool TryGetPending(Guid commandId, out PendingCommand? pendingCommand)
    {
        return _pending.TryGetValue(commandId, out pendingCommand);
    }

    public IReadOnlyList<PendingCommand> GetAll()
    {
        return _pending.Values.ToList();
    }

    public void RemoveExpired(TimeSpan maxAge)
    {
        var cutoff = DateTimeOffset.UtcNow - maxAge;

        foreach (var kvp in _pending)
        {
            if (kvp.Value.CreatedAt < cutoff)
            {
                if (_pending.TryRemove(kvp.Key, out var removed))
                {
                    removed.Completion.TrySetCanceled();
                }
            }
        }
    }
}
