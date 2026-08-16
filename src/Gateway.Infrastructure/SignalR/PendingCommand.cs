namespace Gateway.Infrastructure.SignalR;

public class PendingCommand
{
    public Guid CommandId { get; }
    public Guid ConnectorId { get; }
    public DateTimeOffset CreatedAt { get; }
    public TaskCompletionSource<CommandResultEnvelope> Completion { get; }

    public PendingCommand(Guid commandId, Guid connectorId)
    {
        CommandId = commandId;
        ConnectorId = connectorId;
        CreatedAt = DateTimeOffset.UtcNow;
        Completion = new TaskCompletionSource<CommandResultEnvelope>(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
