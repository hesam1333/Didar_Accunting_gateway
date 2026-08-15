using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gateway.Application.Services.Abstractions;
using Gateway.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private const string ConnectorIdClaimType = "ConnectorId";
    private const string BizDomainIdClaimType = "BizDomainId";
    private const string ProviderClaimType = "Provider";

    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly TimeSpan _tokenLifetime;

    public JwtTokenService(IConfiguration configuration)
    {
        _issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");

        _audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

        var keyValue = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));

        var lifetimeInMinutes = int.TryParse(configuration["Jwt:LifetimeInMinutes"], out var minutes)
            ? minutes
            : 45;

        _tokenLifetime = TimeSpan.FromMinutes(lifetimeInMinutes);
    }

    public string GenerateAgentToken(Connector connector)
    {
        var now = DateTimeOffset.UtcNow;

        var claims = new[]
        {
            new Claim(ConnectorIdClaimType, connector.Id.ToString()),
            new Claim(BizDomainIdClaimType, connector.BizDomainId.ToString()),
            new Claim(ProviderClaimType, connector.Provider.ToString())
        };

        var signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            now.UtcDateTime,
            now.Add(_tokenLifetime).UtcDateTime,
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
