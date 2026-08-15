using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gateway.Api.Filters;

public class ConnectorManagementSecretFilter : IAuthorizationFilter
{
    private const string HeaderName = "auth-con-manager";

    private readonly IConfiguration _configuration;

    public ConnectorManagementSecretFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var expected = _configuration["ConnectorManagement:Secret"];

        if (string.IsNullOrWhiteSpace(expected))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return;
        }

        var headerValue = context.HttpContext.Request.Headers[HeaderName].ToString();

        if (!SecretsMatch(headerValue, expected))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private static bool SecretsMatch(string actual, string expected)
    {
        var actualBytes = Encoding.UTF8.GetBytes(actual);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        return actualBytes.Length == expectedBytes.Length
            && CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
    }
}
