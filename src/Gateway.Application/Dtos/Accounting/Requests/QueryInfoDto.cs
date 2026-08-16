namespace Gateway.Application.Dtos.Accounting.Requests;

public record QueryInfoDto
{
    public string SortBy { get; init; } = string.Empty;

    public bool SortDesc { get; init; }

    public int Take { get; init; }

    public int Skip { get; init; }

    public IReadOnlyList<QueryFilterDto> Filters { get; init; } = new List<QueryFilterDto>();
}
