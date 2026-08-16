using Gateway.Application.Commands;
using Gateway.Application.Dtos.Accounting.Requests;
using Gateway.Application.Dtos.Accounting.Responses;
using Gateway.Application.Services.Abstractions;

namespace Gateway.Application.Services;

public class AccountingService : IAccountingService
{
    private readonly IConnectorManagementService _connectorManagementService;
    private readonly IConnectorCommandDispatcher _commandDispatcher;

    public AccountingService(
        IConnectorManagementService connectorManagementService,
        IConnectorCommandDispatcher commandDispatcher)
    {
        _connectorManagementService = connectorManagementService;
        _commandDispatcher = commandDispatcher;
    }

    public async Task<AccountingResponseBase<BusinessInfoResponse>> GetBusinessInfoAsync(GetBusinessInfoRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<BusinessInfoResponse>(connectorId, new GetBusinessInfoCommand(), cancellationToken);

        return AccountingResponseBase<BusinessInfoResponse>.FromCommandResult(result);
    }

    public async Task<AccountingResponseBase<IReadOnlyList<ProjectResponse>>> GetProjectsAsync(GetProjectsRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<IReadOnlyList<ProjectResponse>>(connectorId, new GetProjectsCommand(), cancellationToken);

        return AccountingResponseBase<IReadOnlyList<ProjectResponse>>.FromCommandResult(result);
    }

    public async Task<AccountingResponseBase<ContactResponse>> GetContactAsync(GetContactRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<ContactResponse>(connectorId, new GetContactCommand(request.Code), cancellationToken);

        return AccountingResponseBase<ContactResponse>.FromCommandResult(result);
    }

    public async Task<AccountingResponseBase<PaginationResponse<ContactResponse>>> SearchContactsAsync(SearchContactsRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<PaginationResponse<ContactResponse>>(connectorId, new SearchContactsCommand(request.QueryInfo), cancellationToken);

        return AccountingResponseBase<PaginationResponse<ContactResponse>>.FromCommandResult(result);
    }

    public async Task<AccountingResponseBase<ContactResponse>> SaveContactAsync(SaveContactRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<ContactResponse>(connectorId, new SaveContactCommand(request.Contact), cancellationToken);

        return AccountingResponseBase<ContactResponse>.FromCommandResult(result);
    }

    public async Task<AccountingResponseBase<InvoiceResponse>> GetInvoiceAsync(GetInvoiceRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<InvoiceResponse>(connectorId, new GetInvoiceCommand(request.Number, request.Type), cancellationToken);

        return AccountingResponseBase<InvoiceResponse>.FromCommandResult(result);
    }

    public async Task<AccountingResponseBase<InvoiceResponse>> SaveInvoiceAsync(SaveInvoiceRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<InvoiceResponse>(connectorId, new SaveInvoiceCommand(request.Invoice), cancellationToken);

        return AccountingResponseBase<InvoiceResponse>.FromCommandResult(result);
    }

    public async Task<AccountingResponseBase<bool>> DeleteInvoiceAsync(DeleteInvoiceRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<bool>(connectorId, new DeleteInvoiceCommand(request.Number, request.Type), cancellationToken);

        return AccountingResponseBase<bool>.FromCommandResult(result);
    }

    public async Task<AccountingResponseBase<ItemResponse>> GetItemAsync(GetItemRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<ItemResponse>(connectorId, new GetItemCommand(request.Code), cancellationToken);

        return AccountingResponseBase<ItemResponse>.FromCommandResult(result);
    }

    public async Task<AccountingResponseBase<PaginationResponse<ItemResponse>>> SearchItemsAsync(SearchItemsRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<PaginationResponse<ItemResponse>>(connectorId, new SearchItemsCommand(request.QueryInfo), cancellationToken);

        return AccountingResponseBase<PaginationResponse<ItemResponse>>.FromCommandResult(result);
    }

    public async Task<AccountingResponseBase<IReadOnlyList<ItemResponse>>> BatchSaveItemsAsync(BatchSaveItemsRequest request, CancellationToken cancellationToken)
    {
        var connectorId = await _connectorManagementService.AuthenticateAsync(request, cancellationToken);

        var result = await _commandDispatcher.DispatchAsync<IReadOnlyList<ItemResponse>>(connectorId, new BatchSaveItemsCommand(request.Items), cancellationToken);

        return AccountingResponseBase<IReadOnlyList<ItemResponse>>.FromCommandResult(result);
    }
}
