namespace Gateway.Application.Commands;

public sealed record CommandResult<TData>
{
    public Guid CommandId { get; init; }

    public bool Success { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public TData? Data { get; init; }

    public static CommandResult<TData> Succeeded(Guid commandId, TData data)
    {
        return new CommandResult<TData>
        {
            CommandId = commandId,
            Success = true,
            Data = data
        };
    }

    public static CommandResult<TData> Failed(Guid commandId, string errorCode, string errorMessage)
    {
        return new CommandResult<TData>
        {
            CommandId = commandId,
            Success = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };
    }
}
