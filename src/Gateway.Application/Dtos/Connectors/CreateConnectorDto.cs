using Gateway.Domain.Enums;

namespace Gateway.Application.Dtos.Connectors;

public record CreateConnectorDto(Guid BizDomainId, ProviderType Provider);
