using Gateway.Application.Commands;

namespace Gateway.Application.Dtos.Accounting.Responses;

public record AccountingResponseBase<TResult>
{
    public bool Success { get; init; }

    public int ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public TResult? Result { get; init; }

    public static AccountingResponseBase<TResult> FromCommandResult(CommandResult<TResult> commandResult)
    {
        return new AccountingResponseBase<TResult>
        {
            Success = commandResult.Success,
            ErrorCode = commandResult.Success ? 0 : ParseErrorCode(commandResult.ErrorCode),
            ErrorMessage = commandResult.ErrorMessage,
            Result = commandResult.Data
        };
    }

    public static AccountingResponseBase<TResult> InvalidCredentials()
    {
        return new AccountingResponseBase<TResult>
        {
            Success = false,
            ErrorCode = 401,
            ErrorMessage = "Invalid accounting credentials",
            Result = default
        };
    }

    private static int ParseErrorCode(string? errorCode)
    {
        return int.TryParse(errorCode, out var parsed) ? parsed : 0;
    }
}
