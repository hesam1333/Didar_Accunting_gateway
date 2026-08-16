namespace Gateway.Application.Exceptions;

public class CommandDispatchException : Exception
{
    public CommandDispatchException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public CommandDispatchException(string errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }

    public static CommandDispatchException AgentNotConnected(Guid connectorId)
    {
        return new CommandDispatchException(
            "AgentNotConnected",
            $"No active agent connection found for connector '{connectorId}'.");
    }

    public static CommandDispatchException Timeout(Guid connectorId, TimeSpan timeout)
    {
        return new CommandDispatchException(
            "CommandTimeout",
            $"Command to connector '{connectorId}' timed out after {timeout.TotalSeconds:F0} seconds.");
    }

    public static CommandDispatchException TransportFailure(Guid connectorId, Exception innerException)
    {
        return new CommandDispatchException(
            "SignalRTransportFailure",
            $"Failed to send command to connector '{connectorId}' via SignalR.",
            innerException);
    }
}
