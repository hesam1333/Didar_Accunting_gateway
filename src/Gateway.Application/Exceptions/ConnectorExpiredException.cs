namespace Gateway.Application.Exceptions;

public class ConnectorExpiredException : InvalidConnectorCredentialsException
{
    public const string ErrorMessage = "اتصال حسابداری شما منقضی شده";

    public ConnectorExpiredException()
        : base(ErrorMessage)
    {
    }
}
