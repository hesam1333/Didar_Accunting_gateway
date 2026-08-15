namespace Gateway.Application.Dtos.Agents;

public record AgentLoginRequestDto(Guid ConnectorId, string AgentSecret);
