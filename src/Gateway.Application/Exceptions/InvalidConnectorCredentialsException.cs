namespace Gateway.Application.Exceptions;

public class InvalidConnectorCredentialsException : Exception
{
    public InvalidConnectorCredentialsException()
        : base("The provided external API credentials are invalid.")
    {
    }

    public InvalidConnectorCredentialsException(string message)
        : base(message)
    {
    }
}
