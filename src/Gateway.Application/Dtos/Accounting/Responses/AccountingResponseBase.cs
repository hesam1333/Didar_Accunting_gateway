namespace Gateway.Application.Dtos.Accounting.Responses;

public record AccountingResponseBase<TResult>
{
    public bool Success { get; init; }

    public int ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public TResult? Result { get; init; }
}
