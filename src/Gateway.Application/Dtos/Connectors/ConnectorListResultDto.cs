namespace Gateway.Application.Dtos.Connectors;

public record ConnectorListResultDto(
    IReadOnlyList<ConnectorDto> Items,
    int TotalCount,
    int Page,
    int PageSize);
