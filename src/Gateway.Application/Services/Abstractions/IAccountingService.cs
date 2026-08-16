using Gateway.Application.Dtos.Accounting.Requests;
using Gateway.Application.Dtos.Accounting.Responses;

namespace Gateway.Application.Services.Abstractions;

public interface IAccountingService
{
    Task<AccountingResponseBase<BusinessInfoResponse>> GetBusinessInfoAsync(GetBusinessInfoRequest request, CancellationToken cancellationToken);

    Task<AccountingResponseBase<IReadOnlyList<ProjectResponse>>> GetProjectsAsync(GetProjectsRequest request, CancellationToken cancellationToken);

    Task<AccountingResponseBase<ContactResponse>> GetContactAsync(GetContactRequest request, CancellationToken cancellationToken);

    Task<AccountingResponseBase<PaginationResponse<ContactResponse>>> SearchContactsAsync(SearchContactsRequest request, CancellationToken cancellationToken);

    Task<AccountingResponseBase<ContactResponse>> SaveContactAsync(SaveContactRequest request, CancellationToken cancellationToken);

    Task<AccountingResponseBase<InvoiceResponse>> GetInvoiceAsync(GetInvoiceRequest request, CancellationToken cancellationToken);

    Task<AccountingResponseBase<InvoiceResponse>> SaveInvoiceAsync(SaveInvoiceRequest request, CancellationToken cancellationToken);

    Task<AccountingResponseBase<bool>> DeleteInvoiceAsync(DeleteInvoiceRequest request, CancellationToken cancellationToken);

    Task<AccountingResponseBase<ItemResponse>> GetItemAsync(GetItemRequest request, CancellationToken cancellationToken);

    Task<AccountingResponseBase<PaginationResponse<ItemResponse>>> SearchItemsAsync(SearchItemsRequest request, CancellationToken cancellationToken);

    Task<AccountingResponseBase<IReadOnlyList<ItemResponse>>> BatchSaveItemsAsync(BatchSaveItemsRequest request, CancellationToken cancellationToken);
}
