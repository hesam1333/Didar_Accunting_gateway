namespace Gateway.Application.Dtos.Accounting;

public record QueryInfoDto(
    string SortBy,
    bool SortDesc,
    int Take,
    int Skip,
    IReadOnlyList<QueryFilterDto> Filters);
