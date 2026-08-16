namespace Gateway.Application.Services.Abstractions;

public interface IAgentIdentityProvider
{
    IAgentIdentity GetCurrent();
}
