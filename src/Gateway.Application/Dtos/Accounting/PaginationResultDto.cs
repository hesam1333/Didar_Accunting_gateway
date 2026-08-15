namespace Gateway.Application.Dtos.Accounting;

public record PaginationResultDto<T>(
    int TotalCount,
    int FilteredCount,
    int From,
    int To,
    IReadOnlyList<T> List);
