using Gateway.Domain.Entities;

namespace Gateway.Application.Services.Abstractions;

public interface IJwtTokenService
{
    string GenerateAgentToken(Connector connector);
}
