using Gateway.Domain.Enums;

namespace Gateway.Application.Exceptions;

public class ConnectorAlreadyExistsException : Exception
{
    public ConnectorAlreadyExistsException(Guid bizDomainId, ProviderType provider)
        : base($"A connector for BizDomain '{bizDomainId}' and provider '{provider}' already exists.")
    {
        BizDomainId = bizDomainId;
        Provider = provider;
    }

    public Guid BizDomainId { get; }

    public ProviderType Provider { get; }
}
