using Gateway.Application.Dtos.Accounting.Requests;
using Gateway.Application.Dtos.Accounting.Responses;
using Gateway.Application.Exceptions;
using Gateway.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Api.Controllers;

[ApiController]
[Route("{provider}/v1")]
public class AccountingController : ControllerBase
{
    private readonly IAccountingService _accountingService;

    public AccountingController(IAccountingService accountingService)
    {
        _accountingService = accountingService;
    }

    [HttpPost("setting/GetBusinessInfo")]
    [ProducesResponseType(typeof(AccountingResponseBase<BusinessInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBusinessInfoAsync(GetBusinessInfoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.GetBusinessInfoAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("setting/GetProjects")]
    [ProducesResponseType(typeof(AccountingResponseBase<IReadOnlyList<ProjectResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProjectsAsync(GetProjectsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.GetProjectsAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("contact/get")]
    [ProducesResponseType(typeof(AccountingResponseBase<ContactResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetContactAsync(GetContactRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.GetContactAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("contact/getcontacts")]
    [ProducesResponseType(typeof(AccountingResponseBase<PaginationResponse<ContactResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchContactsAsync(SearchContactsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.SearchContactsAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("contact/save")]
    [ProducesResponseType(typeof(AccountingResponseBase<ContactResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SaveContactAsync(SaveContactRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.SaveContactAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("invoice/get")]
    [ProducesResponseType(typeof(AccountingResponseBase<InvoiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInvoiceAsync(GetInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.GetInvoiceAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("invoice/save")]
    [ProducesResponseType(typeof(AccountingResponseBase<InvoiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SaveInvoiceAsync(SaveInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.SaveInvoiceAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("invoice/delete")]
    [ProducesResponseType(typeof(AccountingResponseBase<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteInvoiceAsync(DeleteInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.DeleteInvoiceAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("item/get")]
    [ProducesResponseType(typeof(AccountingResponseBase<ItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetItemAsync(GetItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.GetItemAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("item/getitems")]
    [ProducesResponseType(typeof(AccountingResponseBase<PaginationResponse<ItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchItemsAsync(SearchItemsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.SearchItemsAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("item/batchsave")]
    [ProducesResponseType(typeof(AccountingResponseBase<IReadOnlyList<ItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> BatchSaveItemsAsync(BatchSaveItemsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountingService.BatchSaveItemsAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (InvalidConnectorCredentialsException)
        {
            return Unauthorized();
        }
    }
}
