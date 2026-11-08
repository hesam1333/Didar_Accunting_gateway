using Gateway.Application.Dtos.Accounting.Requests;
using Gateway.Application.Dtos.Accounting.Responses;
using Gateway.Application.Exceptions;
using Gateway.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Api.Controllers;

[ApiController]
[Route("/v1")]
public class AccountingController : ControllerBase
{
    private readonly IAccountingService _accountingService;

    public AccountingController(IAccountingService accountingService)
    {
        _accountingService = accountingService;
    }

    [HttpPost("setting/GetBusinessInfo")]
    [ProducesResponseType(typeof(AccountingResponseBase<BusinessInfoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBusinessInfoAsync(GetBusinessInfoRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.GetBusinessInfoAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("setting/GetProjects")]
    [ProducesResponseType(typeof(AccountingResponseBase<IReadOnlyList<ProjectResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectsAsync(GetProjectsRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.GetProjectsAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("contact/get")]
    [ProducesResponseType(typeof(AccountingResponseBase<ContactResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContactAsync(GetContactRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.GetContactAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("contact/getcontacts")]
    [ProducesResponseType(typeof(AccountingResponseBase<PaginationResponse<ContactResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchContactsAsync(SearchContactsRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.SearchContactsAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("contact/save")]
    [ProducesResponseType(typeof(AccountingResponseBase<ContactResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveContactAsync(SaveContactRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.SaveContactAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("invoice/get")]
    [ProducesResponseType(typeof(AccountingResponseBase<InvoiceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoiceAsync(GetInvoiceRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.GetInvoiceAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("invoice/save")]
    [ProducesResponseType(typeof(AccountingResponseBase<InvoiceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveInvoiceAsync(SaveInvoiceRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.SaveInvoiceAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("invoice/delete")]
    [ProducesResponseType(typeof(AccountingResponseBase<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteInvoiceAsync(DeleteInvoiceRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.DeleteInvoiceAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("item/get")]
    [ProducesResponseType(typeof(AccountingResponseBase<ItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItemAsync(GetItemRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.GetItemAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("item/getitems")]
    [ProducesResponseType(typeof(AccountingResponseBase<PaginationResponse<ItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchItemsAsync(SearchItemsRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.SearchItemsAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("item/batchsave")]
    [ProducesResponseType(typeof(AccountingResponseBase<IReadOnlyList<ItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BatchSaveItemsAsync(BatchSaveItemsRequest request, CancellationToken cancellationToken)
    {
        var response = await ExecuteAsync(
            () => _accountingService.BatchSaveItemsAsync(request, cancellationToken),
            cancellationToken);

        return Ok(response);
    }

    private static async Task<AccountingResponseBase<TResult>> ExecuteAsync<TResult>(
        Func<Task<AccountingResponseBase<TResult>>> action,
        CancellationToken cancellationToken)
    {
        try
        {
            return await action();
        }
        catch (ConnectorExpiredException)
        {
            return AccountingResponseBase<TResult>.ConnectorExpired();
        }
        catch (InvalidConnectorCredentialsException)
        {
            return AccountingResponseBase<TResult>.InvalidCredentials();
        }
        catch (CommandDispatchException exception)
        {
            return exception.ErrorCode switch
            {
                "AgentNotConnected" => AccountingResponseBase<TResult>.Failed(
                    StatusCodes.Status503ServiceUnavailable,
                    "ارتباط با نرم‌افزار حسابداری برقرار نیست."),
                "CommandTimeout" => AccountingResponseBase<TResult>.Failed(
                    StatusCodes.Status504GatewayTimeout,
                    "نرم‌افزار حسابداری در زمان تعیین‌شده پاسخی ارسال نکرد."),
                _ => AccountingResponseBase<TResult>.Failed(
                    StatusCodes.Status503ServiceUnavailable,
                    "هنگام ارتباط با نرم‌افزار حسابداری خطایی رخ داد.")
            };
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return AccountingResponseBase<TResult>.Failed(
                StatusCodes.Status500InternalServerError,
                "هنگام پردازش درخواست خطای غیرمنتظره‌ای رخ داد.");
        }
    }
}
