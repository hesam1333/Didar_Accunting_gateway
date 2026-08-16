using Gateway.Domain.Enums;

namespace Gateway.Infrastructure.SignalR;

public record ConnectorCommandEnvelope(
    Guid CommandId,
    CommandType CommandType,
    object Payload);
