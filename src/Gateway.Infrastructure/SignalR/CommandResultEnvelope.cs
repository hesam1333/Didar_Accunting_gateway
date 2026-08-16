namespace Gateway.Infrastructure.SignalR;

public record CommandResultEnvelope(
    Guid CommandId,
    bool Success,
    string? ErrorCode,
    string? ErrorMessage,
    object? Data);
