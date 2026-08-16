namespace Gateway.Application.Dtos.Accounting.Responses;

public record PaginationResponse<T>
{
    public int TotalCount { get; init; }

    public int FilteredCount { get; init; }

    public int From { get; init; }

    public int To { get; init; }

    public IReadOnlyList<T> List { get; init; } = new List<T>();
}
