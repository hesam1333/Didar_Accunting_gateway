using Gateway.Application.Dtos.Connectors;
using Gateway.Infrastructure.Authentication;
using Gateway.Application.Exceptions;
using Gateway.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Api.Controllers;

[ApiController]
[Route("api/connectors")]
[ServiceFilter(typeof(ConnectorManagementSecretFilter))]
public class ConnectorManagementController : ControllerBase
{
    private readonly IConnectorManagementService _connectorManagementService;

    public ConnectorManagementController(IConnectorManagementService connectorManagementService)
    {
        _connectorManagementService = connectorManagementService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ConnectorDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync(
        CreateConnectorDto request,
        CancellationToken cancellationToken)
    {
        var connector = await _connectorManagementService.CreateAsync(request, cancellationToken);

        return Ok(connector);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ConnectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var connector = await _connectorManagementService.GetAsync(id, cancellationToken);

            return Ok(connector);
        }
        catch (ConnectorNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(ConnectorListResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _connectorManagementService.ListAsync(page, pageSize, cancellationToken);

        return Ok(result);
    }

    [HttpGet("biz-domain/{bizDomainId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<ConnectorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListByBizDomainIdAsync(Guid bizDomainId, CancellationToken cancellationToken)
    {
        var connectors = await _connectorManagementService.ListByBizDomainIdAsync(bizDomainId, cancellationToken);

        return Ok(connectors);
    }

    [HttpPut("{id:guid}/login-token/rotate")]
    [ProducesResponseType(typeof(ConnectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RotateLoginTokenAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var connector = await _connectorManagementService.RotateLoginTokenAsync(id, cancellationToken);

            return Ok(connector);
        }
        catch (ConnectorNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:guid}/agent-secret/rotate")]
    [ProducesResponseType(typeof(ConnectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RotateAgentSecretAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var connector = await _connectorManagementService.RotateAgentSecretAsync(id, cancellationToken);

            return Ok(connector);
        }
        catch (ConnectorNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:guid}/enable")]
    [ProducesResponseType(typeof(ConnectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnableAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var connector = await _connectorManagementService.EnableAsync(id, cancellationToken);

            return Ok(connector);
        }
        catch (ConnectorNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:guid}/disable")]
    [ProducesResponseType(typeof(ConnectorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DisableAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var connector = await _connectorManagementService.DisableAsync(id, cancellationToken);

            return Ok(connector);
        }
        catch (ConnectorNotFoundException)
        {
            return NotFound();
        }
    }
}
