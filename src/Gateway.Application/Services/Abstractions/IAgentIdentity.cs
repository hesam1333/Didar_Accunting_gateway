namespace Gateway.Application.Services.Abstractions;

public interface IAgentIdentity
{
    Guid ConnectorId { get; }
    Guid BizDomainId { get; }
}
