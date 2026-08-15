namespace Gateway.Application.Exceptions;

public class InvalidAgentCredentialsException : Exception
{
    public InvalidAgentCredentialsException()
        : base("The provided agent credentials are invalid.")
    {
    }
}
