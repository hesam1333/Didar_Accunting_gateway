namespace Gateway.Application.Exceptions;

public class ConnectorNotFoundException : Exception
{
    public ConnectorNotFoundException(Guid connectorId)
        : base($"Connector '{connectorId}' was not found.")
    {
        ConnectorId = connectorId;
    }

    public Guid ConnectorId { get; }
}
